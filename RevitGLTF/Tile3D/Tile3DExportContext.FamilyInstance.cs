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
    public partial class Tile3DExportContext:IModelExportContext
    {
        public RenderNodeAction OnInstanceBegin(InstanceNode node)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            var id = node.GetSymbolId();
            var familySymbol = mRevitDocument.GetElement(id) as FamilySymbol;
            log.Info(String.Format("Instance FamyliySysmbol:{0} FamilyName:{1} Transform:{2} => Start", 
                familySymbol.Name,
                familySymbol.FamilyName,
                node.GetTransform().Origin.ToString())); 
            return RenderNodeAction.Skip;
        }

        public void OnInstanceEnd(InstanceNode node)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            var id = node.GetSymbolId();
            var familySymbol = mRevitDocument.GetElement(id) as FamilySymbol;
            log.Info(String.Format("Instance FamyliySysmbol:{0} FamilyName:{1} Transform:{2} => End",
                familySymbol.Name,
                familySymbol.FamilyName,
                node.GetTransform().Origin.ToString()));
        }
    }
}
