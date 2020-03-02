using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Tile3DExport.Entities.BatchTable
{
    [DataContract]
    public class BinaryBodyReference
    {
        [DataMember(IsRequired = true)]
        public uint byteOffset { get; set; } = 0;

        public enum ComponentType
        { BYTE, UNSIGNED_BYTE, SHORT, UNSIGNED_SHORT, INT, UNSIGNED_INT, FLOAT, DOUBLE }
        [DataMember(IsRequired = true)]
        public ComponentType componentType { get; set; }

        public enum Type
        { SCALAR, VEC2, VEC3, VEC4 }

        [DataMember(IsRequired = true)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Type type { get; set; }
    }
}
