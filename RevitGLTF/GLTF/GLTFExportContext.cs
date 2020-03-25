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
        //是否取消
        public bool Cancel { get; set; } = false;

        //当前Revit文件
        private Document mRevitDocument = null;

        private View mExportView = null;

        //Element栈,记录Element出栈入栈信息
        private Stack<ElementId> mElementStack = new Stack<ElementId>();

        //构建Mesh堆栈，当前顶点写入Mesh
        private Stack<BabylonMesh> mMeshStack = new Stack<BabylonMesh>();

        //GLTF导出，包含GLTFScene
        private BabylonExportManager mExportManager;

        private BabylonMesh mRootNode = null;

        public GLTFExportContext(BabylonExportManager exportManager, View view/*Document document*/)
        {
            mExportView = view;
            mRevitDocument = view.Document;
            mExportManager = exportManager;
        }

        //开始导出
        public bool Start()
        {
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info(String.Format("Exporting {0} => Start", mRevitDocument.Title));

            //初始化工厂
            InstanceFactory.Instance.Clear();
            MaterialFactory.Instance.Clear();

            //初始化根节点
            mRootNode = new BabylonMesh { name = "root", id = "rootTrans" };
            mRootNode.isDummy = true;
            //GLTFUtil.ExportTransform(mRootNode, Transform.Identity);
            mRootNode.rotation = new float[] { (float)-Math.PI / 2, 0, 0 };
            mExportManager.Scene.MeshesList.Add(mRootNode);

            return true;
        }

        //结束导出
        public void Finish()
        {
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info(String.Format("Exporting {0} => Finish", mRevitDocument.Title));
        }

        //导出过程是否取消
        public bool IsCanceled()
        {
            if(Cancel)
                return true;
            return false;
        }

        //开始导出视图
        public RenderNodeAction OnViewBegin(ViewNode node)
        {
        #if DEBUG
            var view = mRevitDocument.GetElement(node.ViewId) as View;
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            if (view == null)
                log.Error(String.Format("View3D:{0} => Start Failed", node.ViewId.ToString()));
            else
                log.Info(String.Format("View3D {0} => Start", view.Name));
        #endif
            return RenderNodeAction.Proceed;
        }

        //结束视图
        public void OnViewEnd(ElementId elementId)
        {
        #if DEBUG
            var view = mRevitDocument.GetElement(elementId) as View;
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            if (view == null)
                log.Error(String.Format("View3D:{0} => Finish Failed", elementId.ToString()));
            else
                log.Info(String.Format("View3D {0} => Finish", view.Name));
        #endif
        }
    }
}
