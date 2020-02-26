using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace Tile3DExport.Entities
{

    [DataContract]
    public class Properties : Property
    {
        [DataMember(IsRequired = true)]
        public float maximum { get; set; } = 0.0f;

        [DataMember(IsRequired = true)]
        public float minimum { get; set; } = 0.0f;
    }
}
