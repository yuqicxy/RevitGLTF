using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Tile3DExport.Entities.FeatureTable;
using Tile3DExport.Entities.BatchTable;

namespace BabylonTo3DTile
{
    class I3DMHeader : Header
    {
        private const String mMagic = "i3dm";
        
        private const UInt32 mVersion = 1;

        public UInt32 byteLength { get; set; } = 32;// fixed header length

        public UInt32 featureTableJSONByteLength { get; set; }
        
        public UInt32 featureTableBinaryByteLength { get; set; }

        public UInt32 batchTableJSONByteLength { get; set; }

        public UInt32 batchTableBinarybyteLength { get; set; }

        //Indicates the format of the glTF field of the body. 0 indicates it is a uri, 1 indicates it is embedded binary glTF. 
        public UInt32 gltfFormat { get; set; }
        
        public bool Write(BinaryWriter writer)
        {
            writer.Write(mMagic);
            writer.Write(mVersion);
            writer.Write(byteLength);
            writer.Write(featureTableJSONByteLength);
            writer.Write(featureTableBinaryByteLength);
            writer.Write(batchTableJSONByteLength);
            writer.Write(batchTableBinarybyteLength);
            writer.Write(gltfFormat);
            return true;
        }

    }

    class I3DMWriter : TileWriter
    {
        private I3DMHeader header;

        private I3DMFeatureTable featureTable;

        private BatchTable batchTable;

        private bool useExternalGLtf;

        public override uint ComputeLength()
        {
            throw new NotImplementedException();
        }

        public override bool Write(BinaryWriter writer)
        {
            header = new I3DMHeader();

            string featureTableStr = ToJson(featureTable);
            header.featureTableJSONByteLength = (UInt32)featureTableStr.Length;
            header.byteLength += header.featureTableJSONByteLength;

            string batchTableStr = ToJson(batchTable);
            header.batchTableJSONByteLength = (UInt32)batchTableStr.Length;
            header.byteLength += header.batchTableJSONByteLength;

            if (useExternalGLtf)
            {
                header.gltfFormat = 0;
            }
            else
            {
                header.gltfFormat = 1;
            }

            header.Write(writer);
            writer.Write(featureTableStr);
            writer.Write(batchTableStr);

            //writer gltf -- not implement

            throw new NotImplementedException();
        }
    }
}
