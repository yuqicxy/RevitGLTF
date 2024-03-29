﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GLTFExport.Entities
{
    [DataContract]
    public class GLTFNode : GLTFIndexedChildRootProperty
    {
        [DataMember(EmitDefaultValue = false)]
        public int? camera { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int[] children { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int? skin { get; set; }

        // Either matrix or Translation+Rotation+Scale
        //[DataMember]
        //public float[] matrix { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int? mesh { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public float[] translation { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public float[] rotation { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public float[] scale { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public float[] weights { get; set; }

        public List<int> ChildrenList { get; private set; }

        // Used to compute transform world matrix
        public GLTFNode parent;

        public GLTFNode()
        {
            ChildrenList = new List<int>();
        }

        public void Prepare()
        {
            // Do not export empty arrays
            if (ChildrenList.Count > 0)
            {
                children = ChildrenList.ToArray();
            }
        }
    }
}
