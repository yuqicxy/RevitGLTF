using System;
using System.IO;
using System.Runtime.Serialization;

namespace Tile3DExport.Entities
{
    //A tile in a 3D Tiles tileset
    [DataContract]
    public class Tile : Property
    {
        [DataMember(IsRequired = true)]
        public BoundingVolume boundingVolume { get; set; } = new BoundingVolume();

        [DataMember(EmitDefaultValue =false)]
        public BoundingVolume viewerRequestVolume { get; set; }

        [DataMember(IsRequired = true)]
        public float geometricError { get; set; } = 0.0f;

        enum RefinementType {ADD, REPLACE};
        [DataMember(EmitDefaultValue = false)]
        public string refine;

        [DataMember(EmitDefaultValue = false)]
        public float[] transform { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public TileContent content { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Tile[] children { get; set; }
    }
}
