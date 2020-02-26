using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Tile3DExport.Entities
{
    [DataContract]
    public class Tileset : Property
    {
        [DataMember(IsRequired = true)]
        public Asset asset { get; set; } = new Asset();

        [DataMember(EmitDefaultValue =false)]
        public Dictionary<string,Properties> properties { get; set; }

        [DataMember(IsRequired = true)]
        public float geometricError { get; set; } = 0.0f;

        [DataMember(IsRequired = true)]
        public Tile root { get; set; } = new Tile();

        [DataMember(EmitDefaultValue =false)]
        public string[] extensionsUsed { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string[] extensionsRequired { get; set; }
    }
}
