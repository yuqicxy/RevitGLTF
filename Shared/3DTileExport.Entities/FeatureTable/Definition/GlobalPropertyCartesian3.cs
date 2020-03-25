using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Newtonsoft.Json;

namespace Tile3DExport.Entities.FeatureTable
{
    [JsonConverter(typeof(Converter.GlobalPropertyCartesian3Converter))]
    public class GlobalPropertyCartesian3
    {
        public int? offset { get; set; } = null;

        public float[] array { get; set; } = null;
    }

    namespace Converter
    {
        class GlobalPropertyCartesian3Converter : JsonConverter<GlobalPropertyCartesian3>
        {
            public override GlobalPropertyCartesian3 ReadJson(JsonReader reader, Type objectType, /*[AllowNull] */GlobalPropertyCartesian3 existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                throw new NotImplementedException("don't support read GlobalPropertyCartesian3");
            }

            public override void WriteJson(JsonWriter writer, /*[AllowNull] */GlobalPropertyCartesian3 value, JsonSerializer serializer)
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
                    if (value.array.Length != 3)
                    {
                        throw new JsonSerializationException("The length of GlobalPropertyCartesian3 Array Must be 3 instead of " + value.array.Length);
                    }
                    writer.WriteStartArray();
                    foreach (int val in value.array)
                    {
                        writer.WriteValue(val);
                    }
                    writer.WriteEndArray();
                }
                else
                {
                    throw new JsonSerializationException("FAIL:Invalid GlobalPropertyCartesian3");
                }
            }
        }
    }
}
