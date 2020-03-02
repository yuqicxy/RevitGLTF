using System;
using System.Numerics;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Globalization;
using System.IO;
using System.Text;

using Tile3DExport.Entities.FeatureTable;

namespace TestFeatureTable
{
    class TestFeatureTable
    {
        public static string ToJson(Object obj)
        {
            var jsonSerializer = JsonSerializer.Create(new JsonSerializerSettings());
            var sb = new StringBuilder();
            var sw = new StringWriter(sb, CultureInfo.InvariantCulture);

            // Do not use the optimized writer because it's not necessary to truncate values
            // Use the bounded writer in case some values are infinity ()
            using (var jsonWriter = new JsonTextWriter(sw))
            {
#if DEBUG
                jsonWriter.Formatting = Formatting.Indented;
#else
                jsonWriter.Formatting = Formatting.None;
#endif
                jsonSerializer.Serialize(jsonWriter, obj);
            }
            return sb.ToString();
        }
        static void Main(string[] args)
        {
            GlobalPropertyCartesian3 vector3 = new GlobalPropertyCartesian3();
            List<float> val = new List<float>();
            val.Add(1.0f);
            val.Add(1.0f);
            val.Add(1.0f);
            vector3.array = val.ToArray();
            //vector3.offset = 1000;

            GlobalPropertyScalar scalar = new GlobalPropertyScalar();
            //scalar.offset = 10;
            //scalar.array = val.ToArray();
            scalar.number = 10;

            BinaryBodyReference refer = new BinaryBodyReference();
            refer.byteOffset = 10;
            //refer.componentType = Tile3DExport.Entities.BinaryBodyReference.ComponentType.FLOAT;
            var binaryBodyReferencestr = ToJson(refer);
            Console.WriteLine(binaryBodyReferencestr);

            #region B3dmFeatureTable
            B3DMFeatureTable table = new B3DMFeatureTable();
            table.RTC_CENTER = vector3;
            table.BATCH_LENGTH = scalar;
            string tableStr = ToJson(table);
            Console.WriteLine(tableStr);
            #endregion

            #region I3dmFeatureTable
            Console.WriteLine("\nI3dmFeatureTable:");
            I3DMFeatureTable i3dmtable = new I3DMFeatureTable();
            i3dmtable.POSITION = refer;
            i3dmtable.INSTANCES_LENGTH = scalar;
            try
            {
                string i3dmtableStr = ToJson(i3dmtable);
                Console.WriteLine(i3dmtableStr);
            }
            catch (JsonSerializationException exp)
            {
                Console.WriteLine(exp.Message);
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
            #endregion
        }
    }
}
