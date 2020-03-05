using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitGLTF
{
    public class ExportManager
    {
        public ExportManager()
        {
            mExportConfig = new ExportConfig();
        }

        public ExportManager(ExportConfig config)
        {
            mExportConfig = config;
        }

        public void Export(Autodesk.Revit.DB.View exportableView)
        {
            IModelExportContext context = (IModelExportContext)new GLTFExportContext(mExportConfig,exportableView.Document);
            CustomExporter exporter = new CustomExporter(exportableView.Document, context);
            exporter.IncludeGeometricObjects = true;
            exporter.ShouldStopOnError = true;
            try 
            {
                exporter.Export(exportableView);
            }
            //catch(System.IO.IOException exception)//处理 I / O 错误。
            //{
            //
            //}
            //catch(System.IndexOutOfRangeException exception)//处理当方法指向超出范围的数组索引时生成的错误。
            //{
            //}
            //catch(System.ArrayTypeMismatchException exception)//处理当数组类型不匹配时生成的错误。
            //{ }
            //catch(System.NullReferenceException exception) //处理当依从一个空对象时生成的错误。
            //{ }
            //catch(System.DivideByZeroException exception) //处理当除以零时生成的错误。
            //{ }
            //catch(System.InvalidCastException exception) //处理在类型转换期间生成的错误。
            //{ }
            //catch(System.OutOfMemoryException exception) //处理空闲内存不足生成的错误。
            //{ }
            //catch(System.StackOverflowException exception) //处理栈溢出生成的错误。
            //{ }
            catch(System.Exception exception)
            {
                TaskDialog dialog = new TaskDialog("exception");
                dialog.MainContent = exception.Message;
            }
        }

        private ExportConfig mExportConfig;
    }
}
