using System;
using System.Collections.Generic;
using System.Text;

namespace BabylonExport.Entities
{
    public class BoundingVolume
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

		public BoundingVolume() { }
		public BoundingVolume(double minX, double minY, double minZ,
			double maxX, double maxY, double maxZ)
		{
			MinX = minX;
			MinY = minY;
			MaxX = maxX;
			MaxY = maxY;
			MinZ = minZ;
			MaxZ = maxZ;
		}

		public BoundingVolume(float x, float y, float z)
		{
			MinX = x;
			MaxX = x;
			MinY = y;
			MaxY = y;
			MinZ = z;
			MaxZ = z;
		}

		//public bool Equal(BoundingVolume other)
		//{
		//	return other.MinX == this.MinX && other.MinY == this.MinY && other.MinZ == this.MinZ
		//		&& other.MaxX == this.MaxX && other.MaxY == this.MaxY && other.MaxZ == this.MaxZ;
		//}

		//public int Compare(BoundingVolume other)
		//{
		//	if (this.Contains(other))
		//		return 1;

		//	if (other.Contains(this))
		//		return -1;

		//	if(Math.Pow(CenterX,2) + Math.Pow(CenterY,2) + Math.Pow(CenterZ,2) 
		//		< Math.Pow(other.CenterX,2) + Math.Pow(other.CenterY, 2) + Math.Pow(other.CenterZ,2))
		//		return -1;

		//	if (Equal(other))
		//		return 0;

		//	return 1;
		//}

		public void Extend(BoundingVolume other)
		{
			this.MinX = Math.Min(this.MinX, other.MinX);
			this.MinY = Math.Min(this.MinY, other.MinY);
			this.MinZ = Math.Min(this.MinZ, other.MinZ);
			this.MaxX = Math.Max(this.MaxX, other.MaxX);
			this.MaxY = Math.Max(this.MaxY, other.MaxY);
			this.MaxZ = Math.Max(this.MaxZ, other.MaxZ);
		}

		public BoundingVolume Clone()
		{
			return new BoundingVolume
			{
				MinX = this.MinX,
				MinY = this.MinY,
				MinZ = this.MinZ,
				MaxX = this.MaxX,
				MaxY = this.MaxY,
				MaxZ = this.MaxZ,
			};
		}

		public BoundingVolume Intersection(BoundingVolume other)
		{
			return new BoundingVolume
			{
				MinX = Math.Max(this.MinX, other.MinX),
				MinY = Math.Max(this.MinY, other.MinY),
				MinZ = Math.Max(this.MinZ, other.MinZ),
				MaxX = Math.Min(this.MaxX, other.MaxX),
				MaxY = Math.Min(this.MaxY, other.MaxY),
				MaxZ = Math.Min(this.MaxZ, other.MaxZ),
			};
		}

		public BoundingVolume Enlargement(BoundingVolume other)
		{
			var clone = this.Clone();
			clone.Extend(other);
			return clone;
		}

		public bool Contains(BoundingVolume other)
		{
			return
				this.MinX <= other.MinX &&
				this.MinY <= other.MinY &&
				this.MinZ <= other.MinZ &&
				this.MaxX >= other.MaxX &&
				this.MaxY >= other.MaxY &&
				this.MaxZ >= other.MaxZ;
		}

		public bool Intersects(BoundingVolume other)
		{
			return
				this.MinX <= other.MaxX &&
				this.MinY <= other.MaxY &&
				this.MinZ <= other.MaxZ &&
				this.MaxX >= other.MinX &&
				this.MaxY >= other.MinY &&
				this.MaxZ >= other.MinZ;
		}

		public static BoundingVolume InfiniteBounds
		{
			get
			{
				return new BoundingVolume
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


		public static BoundingVolume EmptyBounds
		{
			get
			{
				return new BoundingVolume
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
		public override string ToString()
		{
			return String.Format("Envelope: MinX={0}, MinY={1},MinZ = {2}," +
				"MaxX = {3}, MaxY = {4},MaxZ = {5}", MinX, MinY, MinZ, MaxX, MaxY, MaxZ);
		}
	}
}
