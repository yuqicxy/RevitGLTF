using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Newtonsoft.Json;
namespace Tile3DExport.Entities
{
    [JsonConverter(typeof(Converter.GlobalPropertyScalarConverter))]
    public class GlobalPropertyScalar
    {
        public int? offset { get; set; } = null;

        public float[] array { get; set; } = null;

        public int number { get; set; } = 0;
    }

    namespace Converter
    {
        public class GlobalPropertyScalarConverter : JsonConverter<GlobalPropertyScalar>
        {
            public override GlobalPropertyScalar ReadJson(JsonReader reader, Type objectType, [AllowNull] GlobalPropertyScalar existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                throw new NotImplementedException("don't support read GlobalPropertyScalar");
            }

            public override void WriteJson(JsonWriter writer, [AllowNull] GlobalPropertyScalar value, JsonSerializer serializer)
            {
                if (value.offset != null)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("byteOffset");
                    writer.WriteValue(value.offset);
                    writer.WriteEndObject();
                }
                else if (value.array != null)
                {
                    writer.WriteStartArray();
                    foreach (int val in value.array)
                    {
                        writer.WriteValue(val);
                    }
                    writer.WriteEndArray();
                }
                else
                {
                    writer.WriteValue(value.number);
                }
            }
        }
    }
}
