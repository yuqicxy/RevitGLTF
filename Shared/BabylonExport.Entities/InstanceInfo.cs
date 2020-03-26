using System;
using System.Collections.Generic;
using System.Text;

namespace BabylonExport.Entities
{
    public class InstanceInfo
    {
        public BabylonVector3 Position { get; set; }
        public BabylonVector3 Scale { get; set; }
        public BabylonQuaternion Rotation { get; set; }
    }
}
