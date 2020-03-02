using System;
using System.Numerics;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Globalization;
using System.IO;
using System.Text;

using Newtonsoft.Json;

using Tile3DExport.Entities;

namespace TestTile3D
{
    class TestTile3D
    {
        public static string ToJson(Object obj)
        {
            var jsonSerializer = JsonSerializer.Create(new JsonSerializerSettings());
            var sb = new StringBuilder();
            var sw = new StringWriter(sb, CultureInfo.InvariantCulture);

            // Do not use the optimized writer because it's not necessary to truncate values
            // Use the bounded writer in case some values are infinity ()
            using (var jsonWriter = new JsonTextWriter(sw))
            {
#if DEBUG
                jsonWriter.Formatting = Formatting.Indented;                
#else
                jsonWriter.Formatting = Formatting.None;
#endif
                jsonSerializer.Serialize(jsonWriter, obj);
            }
            return sb.ToString();
        }

        static void Main(string[] args)
        {
            #region Tile3D
            BoundingVolume vol = new BoundingVolume();
            var sphere = new BoundingSphere();
            sphere.Center = new Vector3(0, 0, 0);
            sphere.Radius = 10;
            vol.sphere = sphere.ToArray();

            Tile tile = new Tile();
            tile.boundingVolume = vol;
            TileContent content = new TileContent();
            content.boundingVolume = vol;
            content.uri = "hello.json";
            tile.content = content;
            tile.geometricError = 10;

            Tile childTile = new Tile();
            childTile.boundingVolume = vol;
            childTile.content = content;
            childTile.geometricError = 10;

            List<Tile> tileList = new List<Tile>();
            tileList.Add(childTile);
            tile.children = tileList.ToArray();

            Tileset tileset = new Tileset();
            tileset.root = tile;
            tileset.geometricError = 20;

            String str = ToJson(tileset);
            Console.WriteLine(str);
            #endregion

            #region FeatureTable
            Tile3DExport.Entities.Definition.GlobalPropertyCartesian3 vector3 = new Tile3DExport.Entities.Definition.GlobalPropertyCartesian3();
            List<float> val = new List<float>();
            val.Add(1.0f);
            val.Add(1.0f);
            val.Add(1.0f);
            vector3.array = val.ToArray();
            //vector3.offset = 1000;

            Tile3DExport.Entities.Definition.GlobalPropertyScalar scalar = new Tile3DExport.Entities.Definition.GlobalPropertyScalar();
            scalar.offset = 10;
            scalar.array = val.ToArray();

            Tile3DExport.Entities.b3dm.Featuretable table = new Tile3DExport.Entities.b3dm.Featuretable();
            table.RTC_CENTER = vector3;
            table.BATCH_LENGTH = scalar;
            string tableStr = ToJson(table);
            Console.WriteLine(tableStr);
            #endregion
        }
    }
}
