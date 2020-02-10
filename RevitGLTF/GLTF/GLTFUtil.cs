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

        //public static Matrix4x4 ToFloatArray(Transform matrix)
        //{
        //    XYZ basis0 = matrix.get_Basis(0);
        //    XYZ basis1 = matrix.get_Basis(1);
        //    XYZ basis2 = matrix.get_Basis(2);
        //    XYZ origin = matrix.Origin;

        //    Matrix4x4 array;
        //    array.M11 = (float)basis0.X;
        //    array.M21 = (float)basis0.Y;
        //    array.M31 = (float)basis0.Z;
        //    array.M41 = 0.0f;
        //    array.M12= (float)basis1.X;
        //    array.M22= (float)basis1.Y;
        //    array.M32= (float)basis1.Z;
        //    array.M42= 0.0f;
        //    array.M13= (float)basis2.X;
        //    array.M23= (float)basis2.Y;
        //    array.M33 = (float)basis2.Z;
        //    array.M43 = 0.0f;
        //    array.M14 = (float)origin.X;
        //    array.M24 = (float)origin.Y;
        //    array.M34 = (float)origin.Z;
        //    array.M44 = 0.0f;

        //    return array;
        //}

        //public static Vector3 ToVector3(XYZ vec)
        //{
        //    return new Vector3((float)vec.X, (float)vec.Y, (float)vec.Z);
        //}

        //public static Vector2 ToVector2(UV uv)
        //{
        //    return new Vector2((float)uv.U, (float)uv.V);
        //}

        public static float[] ToArray(XYZ point)
        {
            return new float[] { (float)point.X, (float)point.Y, (float)point.Z };
        }

        public static float[] ToArray(UV point)
        {
            return new float[] { (float)point.U, (float)point.V };
        }
        
        public static float[] ToArray(Color color)
        {
            return new float[] { color.Red, color.Green, color.Blue };
        }

        public static float[] ToArray(Transform matrix)
        {
            XYZ basis0 = matrix.get_Basis(0);
            XYZ basis1 = matrix.get_Basis(1);
            XYZ basis2 = matrix.get_Basis(2);
            XYZ origin = matrix.Origin;
            //return new float[] {(float)basis0.X, (float)basis1.X,(float)basis2.X, (float)origin.X,
            //                    (float)basis0.Y, (float)basis1.Y,(float)basis2.Y, (float)origin.Y,
            //                    (float)basis0.Z, (float)basis1.Z,(float)basis2.Z, (float)origin.Z,
            //                    0.0f    , 0.0f    ,    0.0f, 1.0f};

            return new float[] { (float)basis0.X,(float)basis0.Y,(float)basis0.Z,0.0f,
                          (float)basis1.X,(float)basis1.Y,(float)basis1.Z,0.0f,
                          (float)basis2.X,(float)basis2.Y,(float)basis2.Z,0.0f,
                          (float)origin.X,(float)origin.Y,(float)origin.Z,1.0f};
            
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
            var q = q_babylon;
            float q_length = (float)Math.Sqrt(q.X * q.X + q.Y * q.Y + q.Z * q.Z + q.W * q.W);
            node.rotationQuaternion = new[] { q_babylon.X / q_length, q_babylon.Y / q_length, q_babylon.Z / q_length, q_babylon.W / q_length };
            //scale
            node.scaling = new[] { s_babylon.X, s_babylon.Y, s_babylon.Z };
            //translate
            node.position = new[] { t_babylon.X, t_babylon.Y, t_babylon.Z };
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
