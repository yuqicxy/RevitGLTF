using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

using Tile3DExport.Entities;

namespace Tile3DExport.Entities
{
    [DataContract]
    public class B3DMFeatureTable : Property
    {
        [DataMember(IsRequired = true)]
        public GlobalPropertyScalar BATCH_LENGTH { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GlobalPropertyCartesian3 RTC_CENTER { get; set; }
    }
}
