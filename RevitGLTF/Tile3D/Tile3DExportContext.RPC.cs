﻿using System;
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
        //RPCNode在ModelExportContext中不调用，以polymesh形式出现
        public void OnRPC(RPCNode node)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info(String.Format("RPCNodeName:{0} Transform:{1} => Start",
               node.NodeName,
               node.GetTransform().ToString()));
        }
    }
}
