﻿using System;
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

        private Dictionary<ElementId, BabylonMesh> mInstanceTable = null;

        private InstanceFactory()
        {
            mInstanceTable = new Dictionary<ElementId, BabylonMesh>();
        }

        public void Clear() { mInstanceTable.Clear(); }

        public bool HasCreatedIntance(ElementId symbolId)
        {
            return mInstanceTable.ContainsKey(symbolId);
        }

        public BabylonMesh GetOrCreateInstance(ElementId symbolId)
        {
            if (HasCreatedIntance(symbolId))
            {
                return mInstanceTable[symbolId];
            }
            else 
            {
                BabylonMesh instanceNode = new BabylonMesh();
                instanceNode.id = symbolId.ToString();
                instanceNode.name = symbolId.ToString();
                instanceNode.idGroupInstance = symbolId.IntegerValue;
                instanceNode.isDummy = true;
                mInstanceTable.Add(symbolId, instanceNode);
                return instanceNode;
            }
        }

    }
}
