#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
#endregion

namespace RevitGLTF
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        #region IExternalCommand Members

        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            if (uidoc == null)
            {
                TaskDialog infoDialog = new TaskDialog("警告");
                infoDialog.MainContent = "请打开一个工程";
                infoDialog.Show();
                return Result.Failed;
            }

            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            ExportDialog dialog = new ExportDialog(doc);
            dialog.ShowDialog();

            return Result.Succeeded;
        }
        #endregion
    }
}
