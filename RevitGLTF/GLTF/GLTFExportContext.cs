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
                gltfExporter.ExportGltf(para, mScene, "E:\\RevitPlugin\\RevitGLTF\\RevitGLTF\\bin\\x64\\Log", "1.gltf", false, this);
            }
            catch(Exception e)
            {
                log.Error(e.Message);
                log.Info("Export Fail");
            }

            mScene = null;
            mInstanceDictionary = null;

            string name = "OnViewEnd: Id: " + elementId.IntegerValue;
            log.Info(name);
        }

        public RenderNodeAction OnElementBegin(ElementId elementId)
        {
            mVertices = new List<GlobalVertex>();
            mIndices = new List<int>();
            mCurrentMesh = new BabylonMesh
            {
                id = elementId.ToString(),
                name = elementId.ToString()
            };

            GLTFUtil.ExportTransform(mCurrentMesh, mTransformationStack.Peek());

            var id = elementId.IntegerValue;

            List<int> table = new List<int> {20621,11856, 19020 ,19099,19036,19177,19218};

           if(!table.Contains(id))
           {
               return RenderNodeAction.Skip;
           }

            Element e = mRevitDocument.GetElement(elementId);

            //if (e.Category.Name != "楼板")
            //    return RenderNodeAction.Skip;

            string name = string.Format("OnElementStart: id {0} category {1} name {2}",
                elementId.IntegerValue, e.Category.Name, e.Name);
            log.Info(name);

            return RenderNodeAction.Proceed;
        }

        public void OnElementEnd(ElementId elementId)
        {
            Element e = mRevitDocument.GetElement(elementId);
            string name = string.Format("OnElementEnd: id {0} category {1} name {2}",
                elementId.IntegerValue, e.Category.Name, e.Name);
            log.Info(name);


            if (mIndices.Count() > 0 && mVertices.Count > 0)
            {
                if(mCurrentMesh.subMeshes == null)
                {
                    var subMesh = new BabylonSubMesh();
                    subMesh.indexStart = 0;
                    subMesh.verticesStart = 0;
                    subMesh.indexCount = mIndices.Count();
                    subMesh.verticesCount = mVertices.Count();
                    var subMeshes = new List<BabylonSubMesh>();
                    subMeshes.Add(subMesh);
                    mCurrentMesh.subMeshes = subMeshes.ToArray();
                }


                mCurrentMesh.indices = mIndices.ToArray();
                // Buffers
                mCurrentMesh.positions = mVertices.SelectMany(v => v.Position).ToArray();
                mCurrentMesh.normals = mVertices.SelectMany(v => v.Normal).ToArray();
                float[] firstPosition = mVertices[0].Position;
                bool allEqual = mVertices.All(v => v.Position.IsEqualTo(firstPosition, 0.001f));
                if (allEqual)
                {
                    RaiseWarning("All the vertices share the same position. Is the mesh invisible? The result may not be as expected.", 2);
                }
                mCurrentMesh.uvs = mVertices.SelectMany(v => v.UV).ToArray();
                mScene.MeshesList.Add(mCurrentMesh);
            }

            mVertices = null;
            mIndices = null;
            mCurrentMesh = null;

            //GLTFExporter gltfExporter = new GLTFExporter();
            //ExportParameters para = new ExportParameters();
            //mScene.Prepare(false, false);
            //gltfExporter.ExportGltf(para, mScene, "E:\\RevitPlugin\\RevitGLTF\\RevitGLTF\\bin\\x64\\Log", "1.gltf", false, this);
            //mIsCancel = true;
        }

        public void OnLight(LightNode node)
        {
            Autodesk.Revit.DB.Visual.Asset asset = node.GetAsset();

            string name = node.NodeName + "\tid:" + node.ToString();

            if(asset != null)
                name += "asset:" + asset.Name;

            log.Info("Light\t" + name + "\t");
            //throw new NotImplementedException();
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