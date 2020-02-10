using Utilities;

using Autodesk.Revit.DB;

namespace RevitGLTF
{
    public partial class GLTFExportContext : IModelExportContext,ILoggingProvider
    {
        public void ReportProgressChanged(int progress)
        {
        }

        public void RaiseError(string error, int rank = 0)
        {
        }

        public void RaiseWarning(string warning, int rank = 0)
        {
        }

        public void RaiseMessage(string message, int rank = 0, bool emphasis = false)
        {
        }

        public void RaiseMessage(string message, System.Drawing.Color color, int rank = 0, bool emphasis = false)
        {
        }

        public void RaiseVerbose(string message, int rank = 0, bool emphasis = false)
        {
        }

        public void CheckCancelled()
        {
        }
    }
}