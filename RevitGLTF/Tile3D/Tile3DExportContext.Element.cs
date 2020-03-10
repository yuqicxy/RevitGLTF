using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Babylon2GLTF;
using BabylonExport.Entities;
using Utilities;

using Autodesk.Revit.DB;

namespace RevitGLTF.Tile3D
{
    public partial class Tile3DExportContext : IModelExportContext
    {
        //Element开始
        public RenderNodeAction OnElementBegin(ElementId elementId)
        {
            #region log
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            var element = mRevitDocument.GetElement(elementId);
            if (element == null)
                log.Error(String.Format("Element:{0} =>Start Failed", elementId.ToString()));
            else
                log.Info(String.Format("Element:{0} ID:{1} => Start", element.Name, elementId.ToString()));
            #endregion

            //List<int> table = new List<int> {990620,990621,990645,990646,990647,990648};
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

            //将ElementMesh添加至Scene
            mExportManager.Scene.MeshesList.Add(elementNode);
            
            //同OnInstanceStart处操作，保证无instance的Element正常导出
            //用于合并材质相同的mesh，减少drawcall
            var currentMesh = new MyMesh(elementNode.id);

            //入栈
            mElementStack.Push(elementId);
            mMeshStack.Push(elementNode);
            mMyMeshStack.Push(currentMesh);

            return RenderNodeAction.Proceed;
        }

        //导出element结束
        public void OnElementEnd(ElementId elementId)
        {
            var element = mRevitDocument.GetElement(elementId);
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            if (element == null)
                log.Error(String.Format("Element:{0} =>Finish Failed", elementId.ToString()));
            else
                log.Info(String.Format("Element:{0} ID:{1} => Finish", element.Name, elementId.ToString()));

            BabylonMesh mesh = mMeshStack.Peek();
            var myMesh = mMyMeshStack.Peek();

            if (mesh != null && myMesh != null)
            {
                myMesh.GenerateMesh(mesh);
            }

            //出栈
            mMyMeshStack.Pop();
            mMeshStack.Pop();
            mElementStack.Pop();
        }
    }
}
