using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace MaterialAssetViewer
{
    [Transaction(TransactionMode.Manual)]
    public class GetSelectElementID : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Result result = Result.Succeeded;
            try
            {
                UIDocument revitDoc = commandData.Application.ActiveUIDocument;
                Document dbdoc = revitDoc.Document;

                Autodesk.Revit.DB.View view = dbdoc.ActiveView;
                var selectedIds = commandData.Application.ActiveUIDocument.Selection.GetElementIds();
                String info = "Ids of selected elements in the document are: "; 
                foreach (ElementId id in selectedIds)
                {
                    info += "\n\t" + id.IntegerValue;
                }
                TaskDialog.Show("Revit", info);
            }
            catch(Exception e)
            {
                message = e.Message;
                result = Result.Failed;
            }

            return result;
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class MaterialAssetViewer : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Result result = Result.Succeeded;
            UIDocument revitDoc = commandData.Application.ActiveUIDocument;
            Document dbdoc = revitDoc.Document;

            Autodesk.Revit.DB.View view = dbdoc.ActiveView;
            var selectedIds = commandData.Application.ActiveUIDocument.Selection.GetElementIds();

            if(selectedIds.Count == 0)
            {
                TaskDialog dlg = new TaskDialog("MaterialAssetViewer");
                dlg.MainContent = "未拾取元素";
                dlg.Show();
                result = Result.Failed;
                return result;
            }


            var element = dbdoc.GetElement(selectedIds.First());

            MaterialInfoDialog dialog = new MaterialInfoDialog(element);
            dialog.Show();

            return result;
        }
    }
}
