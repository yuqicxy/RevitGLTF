using System;

namespace RTree
{
	public class Envelope
	{
		public double MinX;
		public double MinY;
        public double MinZ;
        public double MaxX;
		public double MaxY;
		public double MaxZ;
		
		private double? _centerX;
		public double CenterX
		{
			get
			{
				if (Math.Abs(MinX - MaxX) < 0.1)
					_centerX = MinX;
				else
				{
					_centerX = MinX + MaxX * 0.5f;
				}
				return _centerX.Value;
			}
		}

		private double? _centerY;
		public double CenterY
		{
			get
			{
				if (_centerY == null)
				{
					if (Math.Abs(MinY - MaxY) < 0.1)
						_centerY = MinY;
					else
					{
						_centerY = MinY + MaxY * 0.5f;
					}
				}
				return _centerY.Value;
			}
			set { _centerY = value; }
		}

		private double? _centerZ;
		public double CenterZ
        {
            get
            {
                if (_centerZ == null)
                {
                    if (Math.Abs(MinZ - MaxZ) < 0.1)
                        _centerZ = MinZ;
                    else
                    {
                        _centerZ = MinZ + MaxZ * 0.5f;
                    }
                }
                return _centerZ.Value;
            }
            set { _centerZ = value; }
        }

        // NB: Feature `expression bodied members' cannot be used because it is not part of the C# 4.0 language specification
        //public double Area
        //{
        //    get
        //    {
        //        return Math.Max(this.MaxX - this.MinX, 0) * Math.Max(this.MaxY - this.MinY, 0);
        //    }
        //}

		public double Margin
        {
            get
            {
                return Math.Max(this.MaxX - this.MinX, 0) + Math.Max(this.MaxY - this.MinY, 0) + Math.Max(this.MaxZ - this.MinZ, 0);
            }
        }

		public double Volume
        {
            get
            {
				return Math.Max(this.MaxX - this.MinX, 0) * Math.Max(this.MaxY - this.MinY, 0) * Math.Max(this.MaxZ - this.MinZ, 0);
			}
        }

		public Envelope() { }
		public Envelope(double minX, double minY,double minZ,
			double maxX, double maxY,double maxZ)
		{
			MinX = minX;
			MinY = minY;
			MaxX = maxX;
			MaxY = maxY;
			MinZ = minZ;
			MaxZ = maxZ;
		}

		public Envelope(float x, float y,float z)
		{
			MinX = x;
			MaxX = x;
			MinY = y;
			MaxY = y;
			MinZ = z;
			MaxZ = z;
		}

		public void Extend(Envelope other)
		{
			this.MinX = Math.Min(this.MinX, other.MinX);
			this.MinY = Math.Min(this.MinY, other.MinY);
			this.MinZ = Math.Min(this.MinZ, other.MinZ);
			this.MaxX = Math.Max(this.MaxX, other.MaxX);
			this.MaxY = Math.Max(this.MaxY, other.MaxY);
			this.MaxZ = Math.Max(this.MaxZ, other.MaxZ);
		}
		
		public Envelope Clone()
		{
			return new Envelope
			{
				MinX = this.MinX,
				MinY = this.MinY,
                MinZ = this.MinZ,
                MaxX = this.MaxX,
				MaxY = this.MaxY,
				MaxZ = this.MaxZ,
			};
		}

		public Envelope Intersection(Envelope other)
		{
			return new Envelope
			{
				MinX = Math.Max(this.MinX, other.MinX),
				MinY = Math.Max(this.MinY, other.MinY),
                MinZ = Math.Max(this.MinZ, other.MinZ),
                MaxX = Math.Min(this.MaxX, other.MaxX),
				MaxY = Math.Min(this.MaxY, other.MaxY),
                MaxZ = Math.Min(this.MaxZ, other.MaxZ),
            };
		}

		public Envelope Enlargement(Envelope other)
		{
			var clone = this.Clone();
			clone.Extend(other);
			return clone;
		}

		public bool Contains(Envelope other)
		{
			return
				this.MinX <= other.MinX &&
				this.MinY <= other.MinY &&
				this.MinZ <= other.MinZ &&
				this.MaxX >= other.MaxX &&
				this.MaxY >= other.MaxY &&
				this.MaxZ >= other.MaxZ;
		}

		public bool Intersects(Envelope other)
		{
			return
				this.MinX <= other.MaxX &&
				this.MinY <= other.MaxY &&
				this.MinZ <= other.MaxZ &&
				this.MaxX >= other.MinX &&
				this.MaxY >= other.MinY &&
				this.MaxZ >= other.MinZ;
		}

		public static Envelope InfiniteBounds
        {
            get
            {
				return new Envelope
				{
					MinX = double.NegativeInfinity,
					MinY = double.NegativeInfinity,
					MinZ = double.NegativeInfinity,
					MaxX = double.PositiveInfinity,
					MaxY = double.PositiveInfinity,
					MaxZ = double.PositiveInfinity
                };
            }
        }
		

		public static Envelope EmptyBounds
        {
            get
            {
				return new Envelope
				{
					MinX = double.PositiveInfinity,
					MinY = double.PositiveInfinity,
					MinZ = double.PositiveInfinity,
					MaxX = double.NegativeInfinity,
					MaxY = double.NegativeInfinity,
					MaxZ = double.NegativeInfinity
                };
            }
        }

		//public static double Distance(Envelope a, Envelope b)
		//{
		//	var aLength = Math.Sqrt(Math.Pow(a.CenterX, 2) + Math.Pow(a.CenterY, 2));
		//	var bLength = Math.Sqrt(Math.Pow(b.CenterX, 2) + Math.Pow(b.CenterY, 2));
		//	return Math.Abs(aLength - bLength);
		//}

		public override string ToString()
		{
			return String.Format("Envelope: MinX={0}, MinY={1},MinZ = {2}," +
                "MaxX = {3}, MaxY = {4},MaxZ = {5}", MinX, MinY, MinZ,MaxX, MaxY,MaxZ);
		}
	}
}