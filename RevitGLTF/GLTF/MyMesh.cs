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

        private List<int> mIndices = null;
       
        public int MaterialID { get; set; }

        public MySubMesh()
        {
            mVertices = new List<GlobalVertex>();
            mIndices = new List<int>();
        }

        public void AddVertex(GlobalVertex vertex)
        {
            int index = mVertices.IndexOf(vertex);
            if (index == -1)
            {
                mVertices.Add(vertex);
                mIndices.Add(mVertices.Count() - 1);
            }
            else
            {
                mIndices.Add(index);
            }
        }

        public void AddVertexNoIndice(GlobalVertex vertex)
        {
            mVertices.Add(vertex);
        }

        public void AddIndice(int v1,int v2,int v3)
        {
            mIndices.Add(v1);
            mIndices.Add(v2);
            mIndices.Add(v3);
        }

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

        private List<MySubMesh> mSubmeshList;

        public string MaterialID { get; set; }

        public Transform TransformMatrix { get; set; }

        public MyMesh()
        {
            mSubmeshList = new List<MySubMesh>();
        }

        public void AddSubMesh(MySubMesh submesh)
        {
            mSubmeshList.Add(submesh);
        }

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
            mesh.uvs = vertexs.SelectMany(v => v.UV).ToArray();

            mesh.materialId = MaterialID;

            return mesh;
        }
    }
}
