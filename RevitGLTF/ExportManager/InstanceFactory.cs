using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

using BabylonExport.Entities;

namespace RevitGLTF
{
    class InstanceFactory
    {
        private static readonly Lazy<InstanceFactory> lazy = new Lazy<InstanceFactory>(() => new InstanceFactory());

        public static InstanceFactory Instance
        {
            get { return lazy.Value; }
        }

        private Dictionary<int, BabylonMesh> mInstanceTable = null;
        private Dictionary<int, List<InstanceInfo>> mInstanceInfo = null;
        private InstanceFactory()
        {
            mInstanceTable = new Dictionary<int, BabylonMesh>();
            mInstanceInfo = new Dictionary<int, List<InstanceInfo>>();
        }

        public void Clear() { mInstanceTable.Clear(); }

        public bool HasCreatedIntance(ElementId symbolId)
        {
            return mInstanceTable.ContainsKey(symbolId.IntegerValue);
        }

        public BabylonMesh GetOrCreateInstance(ElementId symbolId)
        {
            if (HasCreatedIntance(symbolId))
            {
                return mInstanceTable[symbolId.IntegerValue];
            }
            else 
            {
                BabylonMesh instanceNode = new BabylonMesh();
                instanceNode.id = symbolId.ToString();
                instanceNode.name = symbolId.ToString();
                instanceNode.idGroupInstance = symbolId.IntegerValue;
                instanceNode.isDummy = true;
                mInstanceTable.Add(symbolId.IntegerValue, instanceNode);
                return instanceNode;
            }
        }

        public void AddInstanceInfo(ElementId id, InstanceInfo info)
        {
            if(!mInstanceInfo.ContainsKey(id.IntegerValue))
            { 
                mInstanceInfo[id.IntegerValue] = new List<InstanceInfo>();
            }
            mInstanceInfo[id.IntegerValue].Add(info); 
        }

        public Dictionary<int, List<InstanceInfo>> GetInstanceInfo() { return mInstanceInfo; }

        public Dictionary<int, BabylonMesh> GetInstanceList() { return mInstanceTable; }

    }
}
