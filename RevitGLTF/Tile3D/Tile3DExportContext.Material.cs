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
        public void OnMaterial(MaterialNode node)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            string name = node.NodeName +"\t" + node.GetAppearance().Name  + "\tid:" + node.MaterialId.ToString() + "\tmaterial:" + node.ToString();
            log.Info("MaterialNode\t" + name);
            return;
        }
    }
}
