using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Raylib_cs;
using RpgEngine.Sprites;
using static Raylib_cs.Raylib;

namespace RpgEngine.Renderer
{
    public class Tileset : Sprite
    {
        public Dictionary<int, List<TileProperties>> tileProperties;
        public int columns;
        public int tilecount;
        public int tileheight;
        public int tilewidth;

        public Tileset(dynamic tileset, string imagePath)
            :base(LoadTexture(imagePath))
        {
            this.columns = tileset.columns;
            this.tilecount = tileset.tilecount;
            this.tileheight = tileset.tileheight;
            this.tilewidth = tileset.tilewidth;
            this.tileProperties = new Dictionary<int, List<TileProperties>>();
            try
            {
                foreach (dynamic tile in tileset.tiles)
                {
                    List<TileProperties> properties = new List<TileProperties>();
                    foreach(dynamic prop in tile.properties)
                        properties.Add(new TileProperties((string)prop.name, (string)prop.type, (object)prop.value));
                    tileProperties.Add((int)tile.id, properties);
                }
            } catch { /*tiles got no properties*/ }
        }

        public static Tileset LoadTileset(string jsonFilePath)
        {
            dynamic Loadtileset = null;

            //Load from file
            using(StreamReader r = new StreamReader(jsonFilePath))
            {
                string json = r.ReadToEnd();
                Loadtileset = JsonConvert.DeserializeObject(json);
            }

            return new Tileset(Loadtileset, Path.ChangeExtension(jsonFilePath, Path.GetExtension((string)Loadtileset.image)));
        }

        public void UnloadTileset()
        {
            UnloadSprite();
        }

        public Rectangle ReturnRect(int index)
        {
            //get the x and y coordinates of the tile from a number
            int x = (index % columns) * tilewidth;
            int y = (index / columns) * tileheight;

            return new Rectangle(x, y, tilewidth, tileheight);
        }

        public struct TileProperties
        {
            public string name;
            public string type;
            public dynamic value;

            public TileProperties(string name, string type, object value)
            {
                this.name = name;
                this.type = type;
                this.value = value;
            }
        }
    }
}