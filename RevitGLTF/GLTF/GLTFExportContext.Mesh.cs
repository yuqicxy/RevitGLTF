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

            //if (mInstanceDictionary.ContainsKey(node.GetSymbolId()))
            //{
            //    var babylonInstanceMesh = new BabylonAbstractMesh
            //    {
            //        id = node.GetHashCode().ToString(),
            //        name = node.GetSymbolId().ToString()
            //    };

            //    GLTFUtil.ExportTransform(babylonInstanceMesh, mTransformationStack.Peek());

            //    var mesh = mInstanceDictionary[node.GetSymbolId()];
            //    if (mesh.instances == null)
            //    {
            //        List<BabylonAbstractMesh> list = new List<BabylonAbstractMesh>();
            //        list.Add(babylonInstanceMesh);
            //        mesh.instances = list.ToArray();
            //    }
            //    else 
            //    {
            //        List<BabylonAbstractMesh> list = mesh.instances.ToList();
            //        list.Add(babylonInstanceMesh);
            //        mesh.instances = list.ToArray();
            //    }

            //    return RenderNodeAction.Skip;
            //}

            //mInstanceDictionary[node.GetSymbolId()] = mCurrentMesh;
            //GLTFUtil.ExportTransform(mCurrentMesh, mTransformationStack.Peek());
            //mCurrentMesh.TransformMatrix = mTransformationStack.Peek();

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
            //string name = node.NodeName + "\tid:" + node.ToString();
            //log.Info("FaceNode\t" + name + "\tstart");

            //mCurrentSubMesh = new BabylonSubMesh();
            //mCurrentSubMesh.indexStart = mIndices.Count();
            //mCurrentSubMesh.verticesStart = mVertices.Count();
            return RenderNodeAction.Proceed;
        }

        public void OnFaceEnd(FaceNode node)
        {
            //string name = node.NodeName + "\tid:" + node.ToString();
            //log.Info("FaceNode\t" + name + "\tend");
            
            //var length = mIndices.Count() - mCurrentSubMesh.indexStart;
            //mCurrentSubMesh.indexCount = length;
            //length = mVertices.Count() - mCurrentSubMesh.verticesStart;
            //mCurrentSubMesh.verticesCount = length;
            //if(mCurrentMesh.subMeshes==null)
            //{
            //    var subMeshes = new List<BabylonSubMesh>();
            //    subMeshes.Add(mCurrentSubMesh);
            //    mCurrentMesh.subMeshes = subMeshes.ToArray();
            //}
            //else
            //{
            //    var submeshes = mCurrentMesh.subMeshes.ToList();
            //    submeshes.Add(mCurrentSubMesh);
            //    mCurrentMesh.subMeshes = submeshes.ToArray();
            //}
            //mCurrentSubMesh = null;
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
            //mCurrentMesh.AddSubMesh(mCurrentSubMesh);
            mCurrentSubMesh = null;
            //var length = mIndices.Count() - mCurrentSubMesh.indexStart;
            //mCurrentSubMesh.indexCount = length;
            //length = mVertices.Count() - mCurrentSubMesh.verticesStart;
            //mCurrentSubMesh.verticesCount = length;
            //if (mCurrentMesh.subMeshes == null)
            //{
            //    var subMeshes = new List<BabylonSubMesh>();
            //    subMeshes.Add(mCurrentSubMesh);
            //    mCurrentMesh.subMeshes = subMeshes.ToArray();
            //}
            //else
            //{
            //    var submeshes = mCurrentMesh.subMeshes.ToList();
            //    submeshes.Add(mCurrentSubMesh);
            //    mCurrentMesh.subMeshes = submeshes.ToArray();
            //}
            //mCurrentSubMesh = null;
        }

        public void OnPolymesh2(PolymeshTopology node)
        {
            var lengthPoints = node.NumberOfPoints;
            var lengthNormals = node.NumberOfNormals;
            var lengthUVS = node.NumberOfUVs;
            int iFacet = 0;

            DistributionOfNormals distrib = node.DistributionOfNormals;

            List<GlobalVertex> vertexList = new List<GlobalVertex>();
            for (int i = 0; i < lengthPoints; ++i)
            {
                GlobalVertex v1 = new GlobalVertex();
                v1.Position = GLTFUtil.ToArray(mTransformationStack.Peek().OfPoint(node.GetPoint(i)));
                v1.UV = GLTFUtil.ToArray(node.GetUV(i));
                vertexList.Add(v1);
            }

            foreach (PolymeshFacet triangle in node.GetFacets())
            {
                GlobalVertex v1 = vertexList[triangle.V1];
                GlobalVertex v2 = vertexList[triangle.V2];
                GlobalVertex v3 = vertexList[triangle.V3];

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

                vertexList[triangle.V1] = v1;
                vertexList[triangle.V2] = v2;
                vertexList[triangle.V3] = v3;
                mCurrentSubMesh.AddIndice(triangle.V1, triangle.V2, triangle.V3);
            }

            foreach(var vertex in vertexList)
            {
                mCurrentSubMesh.AddVertexNoIndice(vertex);
            }
        }


        public void OnPolymesh(PolymeshTopology node)
        {
            //OnSubmeshStart();

            //string name = /*node.NodeName + */"\tid:" + node.ToString();
            //log.Info("PolymeshTopology\t" + name);

            //Dictionary<GlobalVertex, List<GlobalVertex>> verticesAlreadyExported = null;
            //if (mConfig.mOptimizeVertices)
            //{
            //    verticesAlreadyExported = new Dictionary<GlobalVertex, List<GlobalVertex>>();
            //}

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

                //if (verticesAlreadyExported != null)
                //{
                //    checkVerticesAlreadyExport(v1, verticesAlreadyExported);
                //    checkVerticesAlreadyExport(v2, verticesAlreadyExported);
                //    checkVerticesAlreadyExport(v3, verticesAlreadyExported);
                //}
                //else 
                //{
                //    mVertices.Add(v1);
                //    mIndices.Add(mVertices.Count() - 1);
                //    mVertices.Add(v2);
                //    mIndices.Add(mVertices.Count() - 1);
                //    mVertices.Add(v3);
                //    mIndices.Add(mVertices.Count() - 1);
                //}
            }

            //OnSubmeshEnd();
        }

        //private bool checkVerticesAlreadyExport(GlobalVertex v, Dictionary<GlobalVertex, List<GlobalVertex>> verticesAlreadyExported)
        //{
        //    if (verticesAlreadyExported == null)
        //        return false;

        //    if (verticesAlreadyExported.ContainsKey(v))
        //    {
        //        verticesAlreadyExported[v].Add(v);
        //        v = verticesAlreadyExported[v].ElementAt(0);
                
        //        mIndices.Add(v.CurrentIndex);
        //        return true;
        //    }
        //    else
        //    {
        //        verticesAlreadyExported[v] = new List<GlobalVertex>();
                
        //        mVertices.Add(v);
        //        v.CurrentIndex = mVertices.Count() - 1;
        //        mIndices.Add(v.CurrentIndex);

        //        verticesAlreadyExported[v].Add(v);
        //        return false;
        //    }
        //}
    }
}
