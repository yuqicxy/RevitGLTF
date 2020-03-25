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
        //外部链接的Revit文档
        public RenderNodeAction OnLinkBegin(LinkNode node)
        {
        #if DEBUG
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info(String.Format("LinkNode:{0}  NodeName:{1}  Transform:{2}=> Start", 
                node.GetDocument().Title, 
                node.NodeName,
                node.GetTransform().Origin.ToString()));
        #endif
            return RenderNodeAction.Skip;
        }

        //导出外部链接结束
        public void OnLinkEnd(LinkNode node)
        {
        }
    }
}
