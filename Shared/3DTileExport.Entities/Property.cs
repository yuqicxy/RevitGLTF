using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Tile3DExport.Entities
{
    [DataContract]
    public class Property
    {
        [DataMember(EmitDefaultValue = false)]
        public Extension extensions { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Dictionary<string, object> extras { get; set; }
    }
}
