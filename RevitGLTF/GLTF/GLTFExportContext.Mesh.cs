using System.Collections.Generic;
using System.Linq;

using Babylon2GLTF;
using BabylonExport.Entities;
using Autodesk.Revit.DB;

namespace RevitGLTF
{
    public partial class GLTFExportContext:IModelExportContext
    {
        private RevitGLTF.MyMesh mCurrentMesh;

        private RevitGLTF.MySubMesh mCurrentSubMesh;

        private Dictionary<int, MySubMesh> mMaterialSubMeshMap;

        //按SymbolID区分是否以导出该实例
        public RenderNodeAction OnInstanceBegin(InstanceNode node)
        {
            string name = node.NodeName + "\tid:" + node.GetSymbolId().ToString() + "\ttype:" + node.GetType().ToString();
            log.Info("InstanceStart\t" + name);

            this.mTransformationStack.Push(this.mTransformationStack.Peek().Multiply(node.GetTransform()));

            return RenderNodeAction.Proceed;
        }

        public void OnInstanceEnd(InstanceNode node)
        {
            mTransformationStack.Pop();

            string name = node.NodeName + "\tid:" + node.GetSymbolId().ToString();
            log.Info("InstanceEnd\t" + name);
        }

        public RenderNodeAction OnFaceBegin(FaceNode node)
        {
            return RenderNodeAction.Proceed;
        }

        public void OnFaceEnd(FaceNode node)
        {          
        }

        private void OnSubmeshStart(int materialID)
        {
            if(mMaterialSubMeshMap.ContainsKey(materialID))
            {
                mCurrentSubMesh = mMaterialSubMeshMap[materialID];
            }
            else 
            {
                mCurrentSubMesh = new RevitGLTF.MySubMesh();
                mCurrentSubMesh.MaterialID = mCurrentMaterialIndex;
                mMaterialSubMeshMap[materialID] = mCurrentSubMesh;
            }
        }

        private void OnSubmeshEnd()
        {
            mCurrentSubMesh = null;
        }     

        public void OnPolymesh(PolymeshTopology node)
        {
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

                v1.Position = GLTFUtil.ToArray(mTransformationStack.Peek().OfPoint(node.GetPoint(triangle.V1)));
                v2.Position = GLTFUtil.ToArray(mTransformationStack.Peek().OfPoint(node.GetPoint(triangle.V2)));
                v3.Position = GLTFUtil.ToArray(mTransformationStack.Peek().OfPoint(node.GetPoint(triangle.V3)));
                
                v1.UV = GLTFUtil.ToArray(node.GetUV(triangle.V1));
                v2.UV = GLTFUtil.ToArray(node.GetUV(triangle.V2));
                v3.UV = GLTFUtil.ToArray(node.GetUV(triangle.V3));

                XYZ normal;
                if (DistributionOfNormals.OnePerFace == distrib)
                {
                    normal = mTransformationStack.Peek().OfVector(node.GetNormal(0));
                    v1.Normal = GLTFUtil.ToArray(normal);
                    v2.Normal = GLTFUtil.ToArray(normal);
                    v3.Normal = GLTFUtil.ToArray(normal);
                }
                else if (DistributionOfNormals.OnEachFacet == distrib)
                {
                    normal = mTransformationStack.Peek().OfVector(node.GetNormal(iFacet++));
                    v1.Normal = GLTFUtil.ToArray(normal);
                    v2.Normal = GLTFUtil.ToArray(normal);
                    v3.Normal = GLTFUtil.ToArray(normal);
                }
                else
                {
                    v1.Normal = GLTFUtil.ToArray(mTransformationStack.Peek().OfVector(node.GetNormal(triangle.V1)));
                    v2.Normal = GLTFUtil.ToArray(mTransformationStack.Peek().OfVector(node.GetNormal(triangle.V2)));
                    v3.Normal = GLTFUtil.ToArray(mTransformationStack.Peek().OfVector(node.GetNormal(triangle.V3)));
                }

                mCurrentSubMesh.AddVertex(v1);
                mCurrentSubMesh.AddVertex(v2);
                mCurrentSubMesh.AddVertex(v3);
            }

        }
    }
}
