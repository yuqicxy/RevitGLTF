using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Tile3DExport.Entities
{
    public class BinaryBodyReference
    {
        [DataMember(IsRequired = true)]
        public uint byteOffset { get; set; } = 0;

        public enum ComponentType
        { BYTE, UNSIGNED_BYTE, SHORT, UNSIGNED_SHORT, INT, UNSIGNED_INT, FLOAT, DOUBLE }
        [DataMember(EmitDefaultValue = false)]
        [JsonConverter(typeof(StringEnumConverter))]
        public ComponentType? componentType { get; set; } = null;
        public bool ShouldSerializecomponentType()
        {
            if (componentType != null)
                return true;
            return false;
        }
    }
}
