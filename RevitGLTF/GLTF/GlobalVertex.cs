using System.Collections.Generic;

namespace RevitGLTF
{
    public struct GlobalVertex: System.IComparable<GlobalVertex>
    {
        public int BaseIndex { get; set; }
        public int CurrentIndex { get; set; }
        public float[] Position { get; set; } // Vec3
        public float[] Normal { get; set; } // Vec3
        public float[] Tangent { get; set; } // Vec3
        public float[] UV { get; set; } // Vec2
        public float[] UV2 { get; set; } // Vec2
        public int BonesIndices { get; set; }
        public float[] Weights { get; set; } // Vec4
        public int BonesIndicesExtra { get; set; }
        public float[] WeightsExtra { get; set; } // Vec4
        public float[] Color { get; set; } // Vec4

        public GlobalVertex(GlobalVertex other)
        {
            this.BaseIndex = other.BaseIndex;
            this.CurrentIndex = other.CurrentIndex;
            this.Position = other.Position;
            this.Normal = other.Normal;
            this.Tangent = other.Tangent;
            this.UV = other.UV;
            this.UV2 = other.UV2;
            this.BonesIndices = other.BonesIndices;
            this.Weights = other.Weights;
            this.BonesIndicesExtra = other.BonesIndicesExtra;
            this.WeightsExtra = other.WeightsExtra;
            this.Color = other.Color;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is GlobalVertex))
            {
                return false;
            }

            var other = (GlobalVertex)obj;

            if (other.BaseIndex != BaseIndex)
            {
                return false;
            }

            if (!other.Position.IsAlmostEqualTo(Position, GLTFUtil.Epsilon))
            {
                return false;
            }

            if (!other.Normal.IsAlmostEqualTo(Normal, GLTFUtil.Epsilon))
            {
                return false;
            }

            if (UV != null && !other.UV.IsAlmostEqualTo(UV, GLTFUtil.Epsilon))
            {
                return false;
            }

            if (UV2 != null && !other.UV2.IsAlmostEqualTo(UV2, GLTFUtil.Epsilon))
            {
                return false;
            }

            if (Weights != null && !other.Weights.IsAlmostEqualTo(Weights, GLTFUtil.Epsilon))
            {
                return false;
            }

            if (WeightsExtra != null && !other.WeightsExtra.IsAlmostEqualTo(WeightsExtra, GLTFUtil.Epsilon))
            {
                return false;
            }

            if (Color != null && !other.Color.IsAlmostEqualTo(Color, GLTFUtil.Epsilon))
            {
                return false;
            }

            return other.BonesIndices == BonesIndices;
        }

        public int CompareTo(GlobalVertex other)
        {
            if (Equals(other))
                return 0;

            if (other.Position != null && Position != null)
            {
                if (Position.Length > other.Position.Length)
                    return 1;
                for (int i = 0; i < Position.Length; i++)
                {
                    if (Position[i] > other.Position[i])
                        return 1;
                }
            }

            if (other.Normal != null && Normal != null)
            {
                if (Normal.Length > other.Normal.Length)
                    return 1;
                for (int i = 0; i < Normal.Length; i++)
                {
                    if (Normal[i] > other.Normal[i])
                        return 1;
                }
            }

            return -1;
        }
    }

    class GlobalVertexLookup : Dictionary<GlobalVertex, int>
    {
        class GlobalVertexEqualityComparer : IEqualityComparer<GlobalVertex>
        {
            public bool Equals(GlobalVertex x, GlobalVertex y)
            {
                return x.Equals(y);
            }

            public int GetHashCode(GlobalVertex obj)
            {
                string str = "";
                for(int i = 0;i < obj.Position.Length; i++)
                {
                    str += obj.Position[i].ToString();
                }
                for (int i = 0; i < obj.Normal.Length; i++)
                {
                    str += obj.Normal[i].ToString();
                }
                return str.GetHashCode();
            }
        }

        public GlobalVertexLookup():base(new GlobalVertexEqualityComparer())
        {
        }
    }
}
