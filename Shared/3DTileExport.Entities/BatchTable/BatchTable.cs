using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace Tile3DExport.Entities.BatchTable
{
    //[DataContract]
    //public class BatchProperty
    //{
    //    [DataMember(EmitDefaultValue = false)]
    //    public BinaryBodyReference ReferenceVal
    //    {
    //        get { return ReferenceVal; }
    //        set
    //        {
    //            if (value != null)
    //            {
    //                ReferenceVal = value;
    //                ArrayVal = null;
    //            }
    //        }
    //    }

    //    [DataMember(EmitDefaultValue = false)]
    //    public Array ArrayVal 
    //    {
    //        get { return ArrayVal; } 
    //        set 
    //        { 
    //            if (value != null) 
    //            {
    //                ArrayVal = value;
    //                ReferenceVal = null;
    //            } 
    //        } 
    //    }
    //}

    [JsonConverter(typeof(Converter.BatchTableConverter))]
    public class BatchTable : Property
    {
        //注意：此处避免无限递归序列化，例如添加batchTable本身进入此Dictionary
        public Dictionary<string, BinaryBodyReference> ReferenceDictionary { get; set; }
        public Dictionary<string, Array> ArrayDictionary { get; set; }
    }

    namespace Converter
    {
        public class BatchTableConverter : JsonConverter<BatchTable>
        {
            public override BatchTable ReadJson(JsonReader reader, Type objectType, /*[AllowNull]*/ BatchTable existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
 
            public override void WriteJson(JsonWriter writer,/* [AllowNull] */BatchTable value, JsonSerializer serializer)
            {
                writer.WriteStartObject();
 
                if (value.ReferenceDictionary != null)
                {
                    foreach (var val in value.ReferenceDictionary)
                    {
                        writer.WritePropertyName(val.Key);
                        serializer.Serialize(writer, val.Value);
                    }
                }

                if (value.ArrayDictionary != null)
                {
                    foreach (var val in value.ArrayDictionary)
                    {
                        writer.WritePropertyName(val.Key);
                        serializer.Serialize(writer, val.Value);
                    }
                }

                if (value.extensions != null)
                {
                    writer.WritePropertyName("extensions");
                    serializer.Serialize(writer, value.extensions);
                }
                if (value.extras != null)
                {
                    writer.WritePropertyName("extras");
                    serializer.Serialize(writer, value.extras);
                }
 
                writer.WriteEndObject();
            }
        }
    }
}
