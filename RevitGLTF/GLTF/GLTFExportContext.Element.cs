using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Babylon2GLTF;
using BabylonExport.Entities;
using Utilities;

using Autodesk.Revit.DB;

namespace RevitGLTF.GLTF
{
    public partial class GLTFExportContext : IModelExportContext
    {
        //Element开始
        public RenderNodeAction OnElementBegin(ElementId elementId)
        {
            var element = mRevitDocument.GetElement(elementId);
#if DEBUG
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            if (element == null)
                log.Error(String.Format("Element:{0} =>Start Failed", elementId.ToString()));
            else
                log.Info(String.Format("Element:{0} ID:{1} => Start", element.Name, elementId.ToString()));
#endif

            //List<int> table = new List<int> {564479,512173, 512277, 512522,512526,550255};
            //List<int> table = new List<int> {976957,976956,976958,976960,976961,977514,977515};
            //if (!table.Contains(elementId.IntegerValue))
            //{
            //    mElementStack.Push(elementId);
            //    mMeshStack.Push(null);
            //    mMyMeshStack.Push(null);
            //    return RenderNodeAction.Skip;
            //}


            //创建ElementMesh
            BabylonMesh elementNode = new BabylonMesh();
            elementNode.id = elementId.ToString();
            elementNode.parentId = mRootNode.id;
            elementNode.isDummy = true;

            if (element != null)
            {
                BoundingBoxXYZ boundBox = element.get_BoundingBox(mExportView);
                if(boundBox != null)
                    elementNode.boundingVolume = GLTFUtil.ToBoundingVolume(boundBox);
            }

            //同OnInstanceStart处操作，保证无instance的Element正常导出
            //用于合并材质相同的mesh，减少drawcall
            var currentMesh = new MyMesh(elementNode.id);

            //入栈
            mElementStack.Push(elementId);
            mMeshStack.Push(elementNode);
            mMyMeshStack.Push(currentMesh);

            //初始化mLastMaterialID
            mLastMaterialID = null;

            return RenderNodeAction.Proceed;
        }

        //导出element结束
        public void OnElementEnd(ElementId elementId)
        {
        #if DEBUG
            var element = mRevitDocument.GetElement(elementId);
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            if (element == null)
                log.Error(String.Format("Element:{0} =>Finish Failed", elementId.ToString()));
            else
                log.Info(String.Format("Element:{0} ID:{1} => Finish", element.Name, elementId.ToString()));
        #endif

            BabylonMesh mesh = mMeshStack.Peek();
            var myMesh = mMyMeshStack.Peek();

            if (mesh != null && myMesh != null)
            {
                myMesh.GenerateMesh(mesh);
                if(!mesh.isDummy)
                {
                    //将ElementMesh添加至Scene
                    mExportManager.Scene.MeshesList.Add(mesh);
                }
            }

            //出栈
            mMyMeshStack.Pop();
            mMeshStack.Pop();
            mElementStack.Pop();
        }
    }
}
