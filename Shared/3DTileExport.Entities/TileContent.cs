using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Tile3DExport.Entities
{
    [DataContract]
    public class TileContent : Property
    {
        [DataMember(EmitDefaultValue = false)]
        public BoundingVolume boundingVolume { set;get;}

        [DataMember(IsRequired = true)]
        public string uri { set; get; }
    }
}
