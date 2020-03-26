using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Globalization;

namespace BabylonTo3DTile
{
    interface Header
    {
        bool Write(BinaryWriter writer);
    }

    abstract class TileWriter
    {
        public abstract bool Write(BinaryWriter writer);

        public abstract UInt32 ComputeLength();

        public static string ToJson(Object obj)
        {
            var jsonSerializer = JsonSerializer.Create(new JsonSerializerSettings());
            var sb = new StringBuilder();
            var sw = new StringWriter(sb, CultureInfo.InvariantCulture);

            // Do not use the optimized writer because it's not necessary to truncate values
            // Use the bounded writer in case some values are infinity ()
            using (var jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.Formatting = Formatting.None;
                jsonSerializer.Serialize(jsonWriter, obj);
            }
            return sb.ToString();
        }
    }
}
