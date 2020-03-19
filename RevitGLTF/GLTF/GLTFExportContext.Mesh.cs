using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Babylon2GLTF;
using BabylonExport.Entities;
using Utilities;

using Autodesk.Revit.DB;
namespace RevitGLTF.GLTF
{
    public partial class GLTFExportContext : IModelExportContext
    {
        private MySubMesh mCurrentSubMesh = null;
        private Stack<MyMesh> mMyMeshStack = new Stack<MyMesh>();

        //Face包含一个polymesh
        public RenderNodeAction OnFaceBegin(FaceNode node)
        {
#if DEBUG
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info(String.Format("FaceNode:{0}  => Start",node.NodeName));
#endif
            return RenderNodeAction.Proceed;
        }

        public void OnFaceEnd(FaceNode node)
        {
#if DEBUG
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info(String.Format("FaceNode:{0}  => End", node.NodeName));
#endif
        }

        //导出顶点 法线 纹理
        public void OnPolymeshReuseVertex(PolymeshTopology node)
        {
#if DEBUG
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info(String.Format("PolymeshTopology =>>>>"));
#endif
            var lengthPoints = node.NumberOfPoints;
            var lengthNormals = node.NumberOfNormals;
            var lengthUVS = node.NumberOfUVs;

            int iFacet = 0;
            DistributionOfNormals distrib = node.DistributionOfNormals;
            foreach (PolymeshFacet triangle in node.GetFacets())
            {
                GlobalVertex v1 = new GlobalVertex();
                GlobalVertex v2 = new GlobalVertex();
                GlobalVertex v3 = new GlobalVertex();

                v1.BaseIndex = triangle.V1;
                v2.BaseIndex = triangle.V2;
                v3.BaseIndex = triangle.V3;

                v1.Position = GLTFUtil.ToArray(node.GetPoint(triangle.V1));
                v2.Position = GLTFUtil.ToArray(node.GetPoint(triangle.V2));
                v3.Position = GLTFUtil.ToArray(node.GetPoint(triangle.V3));

                XYZ normal;
                if (DistributionOfNormals.OnePerFace == distrib)
                {
                    normal = node.GetNormal(0);
                    v1.Normal = GLTFUtil.ToArray(normal);
                    v2.Normal = GLTFUtil.ToArray(normal);
                    v3.Normal = GLTFUtil.ToArray(normal);
                }
                else if (DistributionOfNormals.OnEachFacet == distrib)
                {
                    normal = node.GetNormal(iFacet++);
                    v1.Normal = GLTFUtil.ToArray(normal);
                    v2.Normal = GLTFUtil.ToArray(normal);
                    v3.Normal = GLTFUtil.ToArray(normal);
                }
                else
                {
                    v1.Normal = GLTFUtil.ToArray(node.GetNormal(triangle.V1));
                    v2.Normal = GLTFUtil.ToArray(node.GetNormal(triangle.V2));
                    v3.Normal = GLTFUtil.ToArray(node.GetNormal(triangle.V3));
                }

                if (mCurrentSubMesh.NeedUV)
                {
                    v1.UV = GLTFUtil.ToArray(node.GetUV(triangle.V1));
                    v2.UV = GLTFUtil.ToArray(node.GetUV(triangle.V2));
                    v3.UV = GLTFUtil.ToArray(node.GetUV(triangle.V3));
                }

                mCurrentSubMesh.AddVertex(v1);
                mCurrentSubMesh.AddVertex(v2);
                mCurrentSubMesh.AddVertex(v3);
            }
        }

        public void OnPolymesh(PolymeshTopology node)
        {
#if DEBUG
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info(String.Format("PolymeshTopology =>>>>"));
#endif
            var lengthPoints = node.NumberOfPoints;
            var lengthNormals = node.NumberOfNormals;
            var lengthUVS = node.NumberOfUVs;

            DistributionOfNormals distrib = node.DistributionOfNormals;
            bool normalPerPoint = false;
            if (distrib == DistributionOfNormals.AtEachPoint)
                normalPerPoint = true;

            List<GlobalVertex> vertices = new List<GlobalVertex>();
            for(int i = 0; i < lengthPoints;++i)
            {
                GlobalVertex v = new GlobalVertex();
                v.Position = GLTFUtil.ToArray(node.GetPoint(i));
                v.UV = GLTFUtil.ToArray(node.GetUV(i));

                if (distrib == DistributionOfNormals.AtEachPoint)
                    v.Normal = GLTFUtil.ToArray(node.GetNormal(i));
                else if(distrib == DistributionOfNormals.OnePerFace)
                    v.Normal = GLTFUtil.ToArray(node.GetNormal(0));

                vertices.Add(v);
            }

            List<int> indices = new List<int>();
            int iFacet = 0;
            foreach (PolymeshFacet triangle in node.GetFacets())
            {
                indices.Add(triangle.V1);
                indices.Add(triangle.V2);
                indices.Add(triangle.V3);
                
                if (normalPerPoint)
                    continue;

                XYZ normal;
                GlobalVertex v1 = vertices[triangle.V1];
                GlobalVertex v2 = vertices[triangle.V2];
                GlobalVertex v3 = vertices[triangle.V3];
                if (distrib == DistributionOfNormals.OnEachFacet)
                {
                    normal = node.GetNormal(iFacet++);
                    v1.Normal = GLTFUtil.ToArray(normal);
                    vertices[triangle.V1] = v1;
                    v2.Normal = GLTFUtil.ToArray(normal);
                    vertices[triangle.V2] = v2;
                    v3.Normal = GLTFUtil.ToArray(normal);
                    vertices[triangle.V3] = v3;
                }
            }

            mCurrentSubMesh.AddMesh(indices, vertices);
        }
    }
}
