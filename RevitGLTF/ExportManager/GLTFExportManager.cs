using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Babylon2GLTF;
using BabylonExport.Entities;
using Utilities;

using Autodesk.Revit.DB;

namespace RevitGLTF
{
    class GLTFExportManager : ILoggingProvider
    {
        private ExportConfig mConfig = null;
        private ExportParameters mExportParameters = null;

        public BabylonScene Scene { get; set; }
        public GLTFExportManager(ExportConfig config)
        {
            mConfig = config;
            mExportParameters = new ExportParameters();
            mExportParameters.dracoCompression = true;
            mExportParameters.optimizeVertices = true;

            Scene = new BabylonScene(mConfig.mOutPutPath);
            Scene.producer = new BabylonProducer { name = "Revit", version = "2020" };
        }

        public void Export()
        {
            Export(Scene);
        }

        public void Export(BabylonScene scene)
        {
            try 
            {
                scene.Prepare(false, false);
                GLTFExporter gltfExporter = new GLTFExporter();
                gltfExporter.ExportGltf(mExportParameters, scene, mConfig.mOutPutPath, mConfig.mOutputFilename + mConfig.mOutputFormat, false, this);
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
