using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GLTFExport.Entities;
using Tile3DExport.Entities.FeatureTable;
using Tile3DExport.Entities.BatchTable;

namespace BabylonTo3DTile
{
    class B3DMHeader : Header
    {
        private const String mMagic = "b3dm";

        private const UInt32 mVersion = 1;

        public UInt32 byteLength { get; set; } = 28;// fixed header length

        public UInt32 featureTableJSONByteLength { get; set; }

        public UInt32 featureTableBinaryByteLength { get; set; }

        public UInt32 batchTableJSONByteLength { get; set; }

        public UInt32 batchTableBinarybyteLength { get; set; }

        public bool Write(BinaryWriter writer)
        {
            writer.Write(mMagic);
            writer.Write(mVersion);
            writer.Write(byteLength);
            writer.Write(featureTableJSONByteLength);
            writer.Write(featureTableBinaryByteLength);
            writer.Write(batchTableJSONByteLength);
            writer.Write(batchTableBinarybyteLength);
            return true;
        }
    }

    class B3DMWriter:TileWriter
    {
        private B3DMHeader header;

        private B3DMFeatureTable featureTable;

        private BatchTable batchTable;

        private GLTF gltf;

        private bool dirty = true;

        private String featureTableStr;

        private String batchTableStr;

        private byte[] gltfBinary;

        public override UInt32 ComputeLength()
        {
            if (dirty)
                return header.byteLength;

            header = new B3DMHeader();

            featureTableStr = ToJson(featureTable);
            header.featureTableJSONByteLength = (UInt32)featureTableStr.Length;
            header.byteLength += header.featureTableJSONByteLength;

            batchTableStr = ToJson(batchTable);
            header.batchTableJSONByteLength = (UInt32)batchTableStr.Length;
            header.byteLength += header.batchTableJSONByteLength;

            dirty = true;
            return header.byteLength;
        }

        public override bool Write(BinaryWriter writer)
        {
            if (!dirty)
                ComputeLength();

            header.Write(writer);
            writer.Write(featureTableStr);
            writer.Write(batchTableStr);

            //writer gltf -- not implement
            
            throw new NotImplementedException();
        }
    }
}
