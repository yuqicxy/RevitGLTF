using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Tile3DExport.Entities.Definition
{
    [DataContract]
    public class FeatureTable
    {
        [DataContract]
        protected class BinaryBodyReference
        {
            [DataMember(IsRequired = true)]
            public uint byteOffset { get; set; } = 0;

            enum ComponentType
            { BYTE, UNSIGNED_BYTE, SHORT, UNSIGNED_SHORT, INT, UNSIGNED_INT, FLOAT, DOUBLE }
            [DataMember(EmitDefaultValue = false)]
            public string componentType { get; set; }
        }

        [DataContract]
        protected class FeatureTableProperty
        {
        }

    }
}
