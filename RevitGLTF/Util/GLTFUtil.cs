using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Babylon2GLTF;
using BabylonExport.Entities;

using System.Numerics;
using Autodesk.Revit.DB;

namespace RevitGLTF
{
    static class GLTFUtil
    {
        public const float Epsilon = 0.00001f;

        public static float[] ToArray(XYZ point)
        {
            return new float[] { (float)point.X, (float)point.Y, (float)point.Z };
        }

        public static float[] ToArray(UV point)
        {
            return new float[] { (float)point.U, (float)point.V };
        }      

        public static float[] ToArray(Transform matrix)
        {
            XYZ b0 = matrix.get_Basis(0);
            XYZ b1 = matrix.get_Basis(1);
            XYZ b2 = matrix.get_Basis(2);
            XYZ origin = matrix.Origin;

            //return new float[] { (float)b0.X,(float)b1.X,(float)b2.X,(float)origin.X,
            //                     (float)b0.Y,(float)b1.Y,(float)b2.Y,(float)origin.Y,
            //                     (float)b0.Z,(float)b1.Z,(float)b2.Z,(float)origin.Z,
            //                     0.0f,0.0f,0.0f,1.0f};

            return new float[] { (float)b0.X,(float)b0.Y,(float)b0.Z,0.0f,
                                 (float)b1.X,(float)b1.Y,(float)b1.Z,0.0f,
                                 (float)b2.X,(float)b2.Y,(float)b2.Z,0.0f,
                                 (float)origin.X,(float)origin.Y,(float)origin.Z,1.0f};
        }

        public static BoundingVolume ToBoundingVolume(BoundingBoxXYZ boundBox)
        {
            Transform trf = boundBox.Transform;

            XYZ max = boundBox.Max;
            XYZ min = boundBox.Min;

            XYZ maxInModelCoords = trf.OfPoint(max);
            XYZ minInModelCoords = trf.OfPoint(min);
            return new BoundingVolume(minInModelCoords.X, minInModelCoords.Y, minInModelCoords.Z,
                        maxInModelCoords.X, maxInModelCoords.Y, maxInModelCoords.Z);
        }

        public static void ExportTransform(BabylonNode node, Transform matrix)
        {
            var tm_babylon = new BabylonMatrix();
            tm_babylon.m = ToArray(matrix);

            var s_babylon = new BabylonVector3();
            var q_babylon = new BabylonQuaternion();
            var t_babylon = new BabylonVector3();

            tm_babylon.decompose(s_babylon, q_babylon, t_babylon);
            
            //rotation
            node.rotationQuaternion = q_babylon.ToArray();
            //scale
            node.scaling = s_babylon.ToArray();
            //translate
            node.position = t_babylon.ToArray();
        }

        public static void ExportTransform(InstanceInfo pos, Transform matrix)
        {
            var tm_babylon = new BabylonMatrix();
            tm_babylon.m = ToArray(matrix);

            var s_babylon = new BabylonVector3();
            var q_babylon = new BabylonQuaternion();
            var t_babylon = new BabylonVector3();

            tm_babylon.decompose(s_babylon, q_babylon, t_babylon);

            pos.Position = t_babylon;
            pos.Scale = s_babylon;
            pos.Rotation = q_babylon;
        }

        public static BoundingVolume ExportTransform(BoundingVolume volume,Transform matrix)
        {
            var tm_babylon = new BabylonMatrix();
            tm_babylon.m = ToArray(matrix);

            BabylonVector3 min = new BabylonVector3((float)volume.MinX, (float)volume.MinY, (float)volume.MinZ);
            BabylonVector3 max = new BabylonVector3((float)volume.MaxX, (float)volume.MaxY, (float)volume.MaxZ);

            min = tm_babylon.multiply(min);
            max = tm_babylon.multiply(max);

            return new BoundingVolume(min.X, min.Y, min.Z, max.X, max.Y, max.Z);
        }

        public static BoundingVolume ExportTransform(BoundingVolume volume, BabylonVector3 translation, BabylonQuaternion rotate, BabylonVector3 scale)
        {
            var tm_babylon = BabylonMatrix.Compose(scale, rotate,translation);
            BabylonVector3 min = new BabylonVector3((float)volume.MinX, (float)volume.MinY, (float)volume.MinZ);
            BabylonVector3 max = new BabylonVector3((float)volume.MaxX, (float)volume.MaxY, (float)volume.MaxZ);

            min = tm_babylon.multiply(min);
            max = tm_babylon.multiply(max);

            return new BoundingVolume(min.X, min.Y, min.Z, max.X, max.Y, max.Z);
        }

        public static bool IsEqualTo(this float[] value, float[] other, float Epsilon = Epsilon)
        {
            if (value.Length != other.Length)
            {
                return false;
            }

            return !value.Where((t, i) => Math.Abs(t - other[i]) > Epsilon).Any();
        }
        public static bool IsAlmostEqualTo(this float[] current, float[] other, float epsilon)
        {
            if (current.Length != other.Length)
            {
                return false;
            }

            for (int index = 0; index < current.Length; index++)
            {
                if (Math.Abs(current[index] - other[index]) > epsilon)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
