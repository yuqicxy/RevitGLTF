using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Babylon2GLTF;
using BabylonExport.Entities;
using Utilities;

using Autodesk.Revit.DB;

namespace RevitGLTF
{
    /// <summary>
    /// Export whole rvt file to a gltf/glb File
    /// </summary>
    public partial class GLTFExportContext : IModelExportContext, ILoggingProvider
    {
        bool mIsCancel;

        // Typically, an export context has to manage a stack of transformation
        // for all nested objects, such as instances, lights, links, etc.
        private Stack<Transform> mTransformationStack = new Stack<Transform>();
        private Stack<Document> mDocumentDataStack = new Stack<Document>();

        private Dictionary<ElementId, BabylonMesh> mInstanceDictionary;

        private Document mRevitDocument = null;

        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private BabylonScene mScene = null;

        private ExportConfig mConfig = null;

        BabylonMultiMaterial mCurrentMutiMaterial;

        public GLTFExportContext(ExportConfig config,Document document)
        {
            mConfig = config;

            mIsCancel = false;
            mRevitDocument = document;
            mDocumentDataStack.Push(document);
        }

        public bool Start()
        {
            mTransformationStack = new Stack<Transform>();
            mTransformationStack.Push(Transform.Identity);

            log.Info("Start Export");
            return true;
        }

        public void Finish()
        {
            mTransformationStack.Pop();

            log.Info("End Export");
            return;
        }

        public bool IsCanceled()
        {
            return mIsCancel;
        }

        public Element GetElement(ElementId id)
        {
            return mDocumentDataStack.Peek().GetElement(id);
        }

        public RenderNodeAction OnViewBegin(ViewNode node)
        {
            string name = ("OnViewBegin: "
                + node.NodeName + "(" + node.ViewId.IntegerValue
                + "): LOD: " + node.LevelOfDetail);

            log.Info(name);

            //初始化Scene
            mScene = new BabylonScene(mConfig.mOutPutPath);
            mScene.producer = new BabylonProducer { name = "Revit", version = "2020" };
            mInstanceDictionary = new Dictionary<ElementId, BabylonMesh>();

            return RenderNodeAction.Proceed;
        }

        public void OnViewEnd(ElementId elementId)
        {
            GLTFExporter gltfExporter = new GLTFExporter();
            ExportParameters para = new ExportParameters();
            para.dracoCompression = true;
            para.optimizeVertices = true;

            BabylonMesh rootNode = new BabylonMesh { name = "root",id = "rootTrans"};
            rootNode.isDummy = true;
            //float rootNodeScale = sceneScaleFactor;
            //rootNode.scaling = new float[3] { rootNodeScale, rootNodeScale, rootNodeScale };
            //{
            //    rootNode.rotationQuaternion = new float[] { 0, 0, 0, 1 };
            //}
            {
                rootNode.rotation = new float[] { (float)Math.PI/2, 0, 0 };
            }

            var babylonNodes = new List<BabylonNode>();
            babylonNodes.AddRange(mScene.MeshesList);
            babylonNodes.AddRange(mScene.CamerasList);
            babylonNodes.AddRange(mScene.LightsList);
            foreach (BabylonNode babylonNode in babylonNodes)
            {
                if (babylonNode.parentId == null)
                {
                    babylonNode.parentId = rootNode.id;
                }
            }
            mScene.MeshesList.Add(rootNode);

            mScene.Prepare(false, false);

            try 
            {
                gltfExporter.ExportGltf(para, mScene, mConfig.mOutPutPath,
                    mConfig.mOutputFilename + mConfig.mOutputFormat, false, this);
            }
            catch(Exception e)
            {
                log.Error(e.Message);
                log.Error(e.StackTrace);
                log.Info("Export Fail");
            }

            mScene = null;
            mInstanceDictionary = null;

            string name = "OnViewEnd: Id: " + elementId.IntegerValue;
            log.Info(name);
        }

        public RenderNodeAction OnElementBegin(ElementId elementId)
        {
            Element e = mRevitDocument.GetElement(elementId);
            string name = string.Format("OnElementStart: id {0} category {1} name {2}",
    elementId.IntegerValue, e.Category.Name, e.Name);
            log.Info(name);

            mCurrentMesh = new RevitGLTF.MyMesh();
            mCurrentMesh.ID = elementId.ToString();
            mCurrentMesh.Name = e.Name;
            mCurrentMesh.TransformMatrix = mTransformationStack.Peek();

            mMaterialSubMeshMap = new Dictionary<int, MySubMesh>();

            mCurrentMutiMaterial = new BabylonMultiMaterial();
            mCurrentMutiMaterial.id = elementId.ToString();
            
            mCurrentMaterialID = -1;

            var id = elementId.IntegerValue;

           /*Wood Type Material*/ 
           // List<int> table = new List<int> {20621/*, 20860, 21366, 19735, 19782, 19912, 19861, 34424 */};
           /* Stone Type Material*/ 
           //List<int> table = new List<int> {20621,11856, 19020 ,19099,19036,19177,19218};

           //if (!table.Contains(id))
           //{
           //   return RenderNodeAction.Skip;
           //}

            //if (e.Category.Name != "楼板")
            //    return RenderNodeAction.Skip;

            return RenderNodeAction.Proceed;
        }

        public void OnElementEnd(ElementId elementId)
        {
            Element e = mRevitDocument.GetElement(elementId);
            string name = string.Format("OnElementEnd: id {0} category {1} name {2}",
                elementId.IntegerValue, e.Category.Name, e.Name);
            log.Info(name);

            if (mMaterialSubMeshMap.Count > 0)
            {
                foreach (var submesh in mMaterialSubMeshMap)
                {
                    mCurrentMesh.AddSubMesh(submesh.Value);
                }
                mCurrentMesh.MaterialID = elementId.ToString();
                var babylonMesh = mCurrentMesh.GenerateMesh();
                if (babylonMesh != null)
                {
                    mScene.MeshesList.Add(babylonMesh);
                    mScene.MultiMaterialsList.Add(mCurrentMutiMaterial);
                }
            }

            mCurrentMesh = null;
            mMaterialSubMeshMap = null;
            mCurrentMutiMaterial = null;
        }

        public void OnLight(LightNode node)
        {
            Autodesk.Revit.DB.Visual.Asset asset = node.GetAsset();

            string name = node.NodeName + "\tid:" + node.ToString();

            if(asset != null)
                name += "asset:" + asset.Name;

            log.Info("Light\t" + name + "\t");
        }

        public RenderNodeAction OnLinkBegin(LinkNode node)
        {
            string name = node.NodeName + "\tid:" + node.ToString();
            log.Info("LinkNode\t" + name + "\tstart");
            mTransformationStack.Push(mTransformationStack.Peek().Multiply(node.GetTransform()));
            return RenderNodeAction.Skip;
            return RenderNodeAction.Proceed;
        }

        public void OnLinkEnd(LinkNode node)
        {
            string name = node.NodeName + "\tid:" + node.ToString();
            log.Info("LinkNode\t" + name + "\tend");
            mTransformationStack.Pop();
        }

        public void OnRPC(RPCNode node)
        {
            string name = node.NodeName + "\tid:" + node.ToString();
            log.Info("RPCNode\t" + name);
        }

        public void OnText(TextNode node)
        {
            string name = node.NodeName + "\tid:" + node.ToString();
            log.Info("TextNode\t" + name);
        }
    }
}