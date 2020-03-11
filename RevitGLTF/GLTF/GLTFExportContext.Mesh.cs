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
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info(String.Format("FaceNode:{0}  => Start",node.NodeName));

            return RenderNodeAction.Proceed;
        }

        public void OnFaceEnd(FaceNode node)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info(String.Format("FaceNode:{0}  => End", node.NodeName));
        }

        //导出顶点 法线 纹理
        public void OnPolymesh(PolymeshTopology node)
        {            
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info(String.Format("PolymeshTopology =>>>>"));

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
    }
}
