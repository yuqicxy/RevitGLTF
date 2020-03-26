using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace BabylonTo3DTile
{
    class CMPTHeader : Header
    {
        private const String mMagic = "cmpt";

        private const UInt32 mVersion = 1;

        public UInt32 byteLength { get; set; } = 16;// fixed header length

        public UInt32 tilesLength { get; set; }

        public bool Write(BinaryWriter writer)
        {
            writer.Write(mMagic);
            writer.Write(mVersion);
            writer.Write(byteLength);
            writer.Write(tilesLength);
            return true;
        }
    }
    class CMPTWriter:TileWriter
    {
        private CMPTHeader header;
        
        private List<TileWriter> tiles;

        public override UInt32 ComputeLength()
        {
            throw new NotImplementedException();
        }

        public override bool Write(BinaryWriter writer)
        {
            header = new CMPTHeader();

            foreach (var tile in tiles)
            {
                header.byteLength += tile.ComputeLength();
            }

            header.Write(writer);
            foreach (var tile in tiles)
            {
                tile.Write(writer);
            }

            throw new NotImplementedException();
        }
    }
}
