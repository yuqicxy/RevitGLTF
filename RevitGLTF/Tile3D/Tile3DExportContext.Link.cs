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
        public RenderNodeAction OnLinkBegin(LinkNode node)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info(String.Format("LinkNode:{0}  NodeName:{1}  Transform:{2}=> Start", 
                node.GetDocument().Title, 
                node.NodeName,
                node.GetTransform().Origin.ToString()));

            return RenderNodeAction.Proceed;
        //    throw new NotImplementedException();
        }

        public void OnLinkEnd(LinkNode node)
        {
         //   throw new NotImplementedException();
        }
    }
}
