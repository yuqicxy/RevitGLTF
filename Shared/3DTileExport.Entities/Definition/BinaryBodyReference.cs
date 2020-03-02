using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace Tile3DExport.Entities.Definition
{
    class BinaryBodyReference
    {
        [DataMember(IsRequired = true)]
        public uint byteOffset { get; set; } = 0;

        enum ComponentType
        { BYTE, UNSIGNED_BYTE, SHORT, UNSIGNED_SHORT, INT, UNSIGNED_INT, FLOAT, DOUBLE }
        [DataMember(EmitDefaultValue = false)]
        public string componentType { get; set; }
    }

    class BinaryBodyReferenceConverter : JsonConverter<BinaryBodyReference>
    {
        public override BinaryBodyReference ReadJson(JsonReader reader, Type objectType, [AllowNull] BinaryBodyReference existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, [AllowNull] BinaryBodyReference value, JsonSerializer serializer)
        {
            
            throw new NotImplementedException();
        }
    }
}
