using System;

using Tile3DExport.Entities.BatchTable;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
namespace TestBatchTable
{
    class Address
    {
        public string street { get; set; }
        public string houseNumber { get; set; }
    }

    class TestBatchTable
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

            BinaryBodyReference reference = new BinaryBodyReference();
            reference.byteOffset = 10;
            reference.componentType = BinaryBodyReference.ComponentType.DOUBLE;
            reference.type = BinaryBodyReference.Type.VEC3;

            List<float> list = new List<float>();
            list.Add(1.0f);
            list.Add(2.0f);
            list.Add(3.0f);

            List<Address> adderssList = new List<Address>();
            Address address = new Address { street = "Main Street", houseNumber = "1.0" };
            adderssList.Add(address);
            adderssList.Add(address);

            BatchTable table = new BatchTable();
            table.ReferenceDictionary = new Dictionary<string, BinaryBodyReference>();
            table.ReferenceDictionary.Add("height", reference);
            table.ReferenceDictionary.Add("geographic", reference);
            table.ArrayDictionary = new Dictionary<string, Array>();
            table.ArrayDictionary.Add("yearBuilt", list.ToArray());
            table.ArrayDictionary.Add("address",adderssList.ToArray());

            var str = ToJson(table);
            Console.WriteLine("BatchTable:");
            Console.WriteLine(str);
        }
    }
}
