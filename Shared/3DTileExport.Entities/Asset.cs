using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Tile3DExport.Entities
{
    [DataContract]
    public class Asset : Property
    {
        [DataMember(IsRequired = true)]
        public string version { set; get; } = "1.0";

        [DataMember(EmitDefaultValue = false)]
        public string tilesetVersion { set; get; }
    }
}
