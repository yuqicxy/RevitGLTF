using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Runtime.Serialization;

namespace Tile3DExport.Entities
{
    //Oriented bounding box
    public class BoundingBox
    {
        public Vector3 Center { get; set; }
        public Vector3 XHalfAxis { get; set; }
        public Vector3 YHalfAxis { get; set; }
        public Vector3 ZHalfAxis { get; set; }

        public float[] ToArray()
        {
            List<float> list = new List<float>();
            list.Add(Center.X);
            list.Add(Center.Y);
            list.Add(Center.Z);
            list.Add(XHalfAxis.X);
            list.Add(XHalfAxis.Y);
            list.Add(XHalfAxis.Z);
            list.Add(YHalfAxis.X);
            list.Add(YHalfAxis.Y);
            list.Add(YHalfAxis.Z);
            list.Add(ZHalfAxis.X);
            list.Add(ZHalfAxis.Y);
            list.Add(ZHalfAxis.Z);
            return list.ToArray();
        }
    }

    //An array of six numbers that define a bounding geographic region in EPSG:4979 coordinates
    //with the order [west, south, east, north, minimum height, maximum height]. 
    //Longitudes and latitudes are in radians, and heights are in meters 
    //above (or below) the WGS84 ellipsoid.
    public class BoundingRegion
    {
        public Vector3 WestSouth { get; set; }
        public Vector3 EastNorth { get; set; }

        public float[] ToArray()
        {
            List<float> list = new List<float>();
            list.Add(WestSouth.X);
            list.Add(WestSouth.Y);
            list.Add(EastNorth.X);
            list.Add(EastNorth.Y);
            list.Add(WestSouth.Z);
            list.Add(EastNorth.Z);
            return list.ToArray();
        }

    }

    //An array of four numbers that define a bounding sphere.  
    //The first three elements define the x, y, and z values 
    //for the center of the sphere.  
    //The last element (with index 3) defines the radius in meters.
    public class BoundingSphere
    {
        public Vector3 Center { get; set; }
        public float Radius { get; set; }

        public float[] ToArray()
        {
            List<float> list = new List<float>();
            list.Add(Center.X);
            list.Add(Center.Y);
            list.Add(Center.Z);
            list.Add(Radius);
            return list.ToArray();
        }
    }

    [DataContract]
    public class BoundingVolume : Property
    {
        [DataMember(EmitDefaultValue = false)]
        public float[] box { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public float[] region { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public float[] sphere { get; set; }
    }
}
