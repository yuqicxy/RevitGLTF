using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

using Tile3DExport.Entities;

namespace Tile3DExport.Entities
{
    namespace b3dm
    {
        [DataContract]
        public class Featuretable : Property
        {
            [DataMember(IsRequired = true)]
            public Definition.GlobalPropertyScalar BATCH_LENGTH { get; set; }

            [DataMember(EmitDefaultValue = false)]
            public Definition.GlobalPropertyCartesian3 RTC_CENTER { get; set; }
        }
    }
}
