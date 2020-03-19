using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BabylonExport.Entities;
using Autodesk.Revit.DB;

namespace RevitGLTF
{
    class MySubMesh
    {
        private List<GlobalVertex> mVertices = null;
        private GlobalVertexLookup mLookupTable = null;
        public bool NeedUV { get; set; } = true;

        private List<int> mIndices = null;
       
        public int MaterialID { get; set; }

        public MySubMesh()
        {
            mVertices = new List<GlobalVertex>();
            mLookupTable = new GlobalVertexLookup();
            mIndices = new List<int>();
        }

        //为Intsance复制使用
        public MySubMesh(MySubMesh other,int tmpMaterialID)
        {
            this.mVertices = other.mVertices;
            this.mLookupTable = other.mLookupTable;
            this.mIndices = other.mIndices;
            this.MaterialID = tmpMaterialID;
        }

        public void AddMesh(List<int> indices, List<GlobalVertex> vertexs)
        {
            int size = mVertices.Count();
            mVertices.AddRange(vertexs);

            indices.ForEach(indice => 
            {
                indice += size;
                mIndices.Add(indice);
            });
        }

        //添加顶点至Vertex中，并进行顶点去重，以及顶点索引的构建
        public void AddVertex(GlobalVertex vertex)
        {
            if(!mLookupTable.ContainsKey(vertex))
            {
                mVertices.Add(vertex);
                mIndices.Add(mVertices.Count() - 1);
                mLookupTable.Add(vertex, mVertices.Count() - 1);
            }
            else 
            {
                var index = mLookupTable[vertex];
                mIndices.Add(index);
            }
        }

        //构建babylonSubmesh
        internal BabylonSubMesh Arrange(List<int> indices, List<GlobalVertex> vertexs)
        {
            BabylonSubMesh submesh = new BabylonSubMesh();
            submesh.verticesStart = vertexs.Count();
            submesh.indexStart = indices.Count();

            foreach (var indice in mIndices)
            {
                indices.Add(indice + submesh.verticesStart);
            }
            submesh.indexCount = indices.Count() - submesh.indexStart;

            vertexs.AddRange(mVertices);
            submesh.verticesCount = vertexs.Count() - submesh.verticesStart;

            submesh.materialIndex = MaterialID;

            return submesh;
        }
    }

    class MyMesh
    {
        public string ID { get; set; }

        public string Name{ get; set; }

        //用于合并材质相同的mesh，减少drawcall
        public Dictionary<ElementId, MySubMesh> MaterialSubMeshMap = null;

        //当前MultiMaterial，与BabylonMesh绑定
        public BabylonMultiMaterial MultiMaterial = null;

        private List<MySubMesh> mSubmeshList;

        public string MaterialID { get; set; }

        public Transform TransformMatrix { get; set; }

        public MyMesh()
        {
            mSubmeshList = new List<MySubMesh>();
            MaterialSubMeshMap = new Dictionary<ElementId, MySubMesh>();
            MultiMaterial = new BabylonMultiMaterial();
            List<String> materials = new List<String>();
            MultiMaterial.materials = materials.ToArray();
        }

        public MyMesh(string id)
        {
            ID = id;
            Name = id;
            mSubmeshList = new List<MySubMesh>();
            MaterialSubMeshMap = new Dictionary<ElementId, MySubMesh>();

            MultiMaterial = new BabylonMultiMaterial();
            MultiMaterial.id = ID;

            List<String> materials = new List<String>();
            MultiMaterial.materials = materials.ToArray();
        }

        public void AddSubMesh(MySubMesh submesh)
        {
            mSubmeshList.Add(submesh);
        }

        public void GenerateMesh(BabylonMesh mesh)
        {
            List<int> indices = new List<int>();
            List<GlobalVertex> vertexs = new List<GlobalVertex>();

            foreach (var pair in MaterialSubMeshMap)
            {
                var submesh = pair.Value;
                BabylonSubMesh babylonSubmesh = submesh.Arrange(indices, vertexs);
                if (babylonSubmesh != null)
                {
                    if (mesh.subMeshes == null)
                    {
                        var subMeshes = new List<BabylonSubMesh>();
                        subMeshes.Add(babylonSubmesh);
                        mesh.subMeshes = subMeshes.ToArray();
                    }
                    else
                    {
                        var submeshes = mesh.subMeshes.ToList();
                        submeshes.Add(babylonSubmesh);
                        mesh.subMeshes = submeshes.ToArray();
                    }
                }
            }

            mesh.id = ID;
            mesh.name = Name;
            mesh.materialId = MultiMaterial.id;

            if(TransformMatrix == null)
                TransformMatrix = Transform.Identity;

            GLTFUtil.ExportTransform(mesh, TransformMatrix);

            if (vertexs.Count <= 0)
                return;
            
            mesh.indices = indices.ToArray();
            // Buffers
            mesh.positions = vertexs.SelectMany(v => v.Position).ToArray();
            mesh.normals = vertexs.SelectMany(v => v.Normal).ToArray();
            //float[] firstPosition = vertexs[0].Position;
            //bool allEqual = vertexs.All(v => v.Position.IsEqualTo(firstPosition, 0.001f));
            //if (allEqual)
            //{
            //    log.Warn("All the vertices share the same position." +
            //        " Is the mesh invisible? The result may not be as expected.");
            //}
            if (vertexs.First().UV != null)
                mesh.uvs = vertexs.SelectMany(v => v.UV).ToArray();

            mesh.isDummy = false;
        }

        //构建BabylonMesh
        public BabylonMesh GenerateMesh()
        {
            log4net.ILog log = 
                log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            List<int> indices = new List<int>();
            List<GlobalVertex> vertexs = new List<GlobalVertex>();

            BabylonMesh mesh = new BabylonMesh();
            foreach(var submesh in mSubmeshList)
            {
               BabylonSubMesh babylonSubmesh = submesh.Arrange(indices, vertexs);
               if(babylonSubmesh != null)
                {
                    if (mesh.subMeshes == null)
                    {
                        var subMeshes = new List<BabylonSubMesh>();
                        subMeshes.Add(babylonSubmesh);
                        mesh.subMeshes = subMeshes.ToArray();
                    }
                    else
                    {
                        var submeshes = mesh.subMeshes.ToList();
                        submeshes.Add(babylonSubmesh);
                        mesh.subMeshes = submeshes.ToArray();
                    }
                }
            }

            if (vertexs.Count <= 0)
                return null;

            mesh.id = ID;
            mesh.name = Name;
            mesh.materialId = MaterialID;

            GLTFUtil.ExportTransform(mesh, TransformMatrix);

            mesh.indices = indices.ToArray();
            // Buffers
            mesh.positions = vertexs.SelectMany(v => v.Position).ToArray();
            mesh.normals = vertexs.SelectMany(v => v.Normal).ToArray();
            float[] firstPosition = vertexs[0].Position;
            bool allEqual = vertexs.All(v => v.Position.IsEqualTo(firstPosition, 0.001f));
            if (allEqual)
            {
                log.Warn("All the vertices share the same position." +
                    " Is the mesh invisible? The result may not be as expected.");
            }
            if(vertexs.First().UV != null)
                mesh.uvs = vertexs.SelectMany(v => v.UV).ToArray();

            mesh.materialId = MaterialID;

            return mesh;
        }
    }
}
