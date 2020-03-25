using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

using Babylon2GLTF;
using BabylonExport.Entities;
using Tile3DExport.Entities;
using Utilities;

namespace RevitGLTF
{
    public class BabylonExportManager : ILoggingProvider
    {
        private ExportConfig mConfig = null;

        public BabylonScene Scene { get; set; }

        public BabylonExportManager(ExportConfig config)
        {
            mConfig = config;

            Scene = new BabylonScene(mConfig.mOutPutPath);
            Scene.producer = new BabylonProducer { name = "浙江科澜信息技术有限公司——Revit导出插件", version = "1.0" };
        }
        public ExportConfig GetConfig() { return mConfig; }

        public void Export()
        {
            if (mConfig.mExportMode == ExportConfig.ExportMode.GLTF)
                ExportToGLTF(Scene);
            else
                ExportTo3DTile(Scene);
        }

        public void ExportToGLTF(BabylonScene scene)
        {
            try 
            {
                var exportParameters = new BabylonExport.Entities.ExportParameters();
                exportParameters.dracoCompression = mConfig.mDracoCompress;
                exportParameters.optimizeVertices = true;

                bool generateBinary = mConfig.mOutputFormat == ".glb";
                scene.Prepare(false, false);
                GLTFExporter gltfExporter = new GLTFExporter();
                gltfExporter.ExportGltf(exportParameters, scene, mConfig.mOutPutPath, mConfig.mOutputFilename + mConfig.mOutputFormat, generateBinary, this);
            }
            catch(Exception exp)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                log.Error(exp.Source);
                log.Error(exp.TargetSite.ToString());
                log.Error(exp.Message);
                log.Error(exp.StackTrace);
            }
        }

        public void ExportTo3DTile(BabylonScene scene)
        {
            try
            {
                var exportParameter = new Tile3DExportParameter();
                exportParameter.TilePath = mConfig.mOutPutPath;
                
                Tile3DExporter tile3DExporter = new Tile3DExporter();
                tile3DExporter.ExportTile3D(exportParameter,scene, this);
            }
            catch (Exception exp)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                log.Error(exp.Source);
                log.Error(exp.TargetSite.ToString());
                log.Error(exp.Message);
                log.Error(exp.StackTrace);
            }
        }

        public void CheckCancelled()
        {
        }

        public void RaiseError(string error, int rank = 0)
        {
        #if DEBUG
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Error(error);
        #endif
        }

        public void RaiseMessage(string message, int rank = 0, bool emphasis = false)
        {
        #if DEBUG
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info(message);
        #endif
        }

        public void RaiseMessage(string message, System.Drawing.Color color, int rank = 0, bool emphasis = false)
        {
        #if DEBUG
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info(message);
        #endif
        }

        public void RaiseVerbose(string message, int rank = 0, bool emphasis = false)
        {
        #if DEBUG
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info(message);
        #endif
        }

        public void RaiseWarning(string warning, int rank = 0)
        {
        #if DEBUG
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Warn(warning);
        #endif
        }

        public void ReportProgressChanged(int progress)
        {
        }
    }
}
