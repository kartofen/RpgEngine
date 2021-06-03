using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Xml;
using Newtonsoft.Json;
using Raylib_cs;
using static Raylib_cs.Raylib;


namespace RpgEngine
{
    public class Tilemap
    {
        public Dictionary<int, Tileset> Tilesets = new Dictionary<int, Tileset>();
        public List<Layer> layers = new List<Layer>();
        public int Width;
        public int Height;
        public int TileWidth;
        public int TileHeight;
        
        public Tilemap() { }
        public Tilemap(Dictionary<int, Tileset> dict, int height, int width, int tileHeight, int tileWidth, List<Layer> layers)
        {
            this.Tilesets = dict;
            this.Height = height;
            this.Width = width;
            this.TileHeight = tileHeight;
            this.TileWidth = tileWidth;
            this.layers = layers;
        }

        public static Tilemap LoadTilemap(string path)
        {
            List<Layer> LoadLayers = new List<Layer>();
            Dictionary<int, Tileset> LoadTilesets = new Dictionary<int, Tileset>();
            XmlDocument LoadTilemap = new XmlDocument();
            LoadTilemap.Load(path);

            // load map
            XmlNode map = LoadTilemap.SelectSingleNode("/map");
            var width = map.Attributes["width"].Value;
            var height = map.Attributes["height"].Value;
            var Tilewidth = map.Attributes["tilewidth"].Value;
            var Tileheight = map.Attributes["tileheight"].Value;

            foreach(XmlNode x in map)
            {
                // load tilesets
                if(x.Name == "tileset")
                    LoadTilesets.Add(int.Parse(x.Attributes["firstgid"].Value), Tileset.LoadTileset("Assets/" + x.Attributes["source"].Value));
                // load layers
                else if(x.Name == "layer")
                {
                    List<Chunk> ChunkList = new List<Chunk>();

                    foreach(XmlNode chunk in x.FirstChild)
                    {
                        ChunkList.Add(new Chunk(chunk.InnerText.Split(',').Select(Int32.Parse).ToList(),  new Vector2(int.Parse(chunk.Attributes["x"].Value), int.Parse(chunk.Attributes["y"].Value)), new Vector2(int.Parse(chunk.Attributes["width"].Value), int.Parse(chunk.Attributes["height"].Value))));
                    }

                    //LoadLayers.Add(new Layer(x.FirstChild.InnerText.Split(',').Select(Int32.Parse).ToList(), int.Parse(x.Attributes["height"].Value), int.Parse(x.Attributes["width"].Value)));
                    LoadLayers.Add(new Layer(ChunkList, int.Parse(x.Attributes["height"].Value), int.Parse(x.Attributes["width"].Value)));
                }
            }

            return new Tilemap(LoadTilesets, int.Parse(height), int.Parse(width), int.Parse(Tileheight), int.Parse(Tilewidth), LoadLayers);
        }

        public void UnloadTilemap()
        {
            //unload all tilesets
            foreach(Tileset t in this.Tilesets.Values)
            {
                t.UnloadTileset();
            }
        }

        public void Draw(Vector2 pos)
        {
            //reset collision boxes list and object lists 
            CollisionManager.collisionBoxes.Clear();
            CollisionManager.hitBoxes.Clear();
            ObjectManager.Clear();

            Vector2 position = pos;

            //change when position is negative
            if(position.X < 0) 
                position.X -= (layers[0].Chunks[0].Dimensions.X * TileWidth);
            if(position.Y < 0)
                position.Y -= (layers[0].Chunks[0].Dimensions.Y * TileHeight);
        
            //get which chunk to draw
            Vector2 positionInChunk = new Vector2(
                (int)(((int)(position.X)/TileWidth )/layers[0].Chunks[0].Dimensions.Y)*layers[0].Chunks[0].Dimensions.X,
                (int)(((int)(position.Y)/TileHeight)/layers[0].Chunks[0].Dimensions.Y)*layers[0].Chunks[0].Dimensions.Y
            );

            foreach(Layer l in this.layers)
            {
                //get chunks to draw
                List<Chunk> ChunksToDraw = new List<Chunk>();
                ChunksToDraw.Add(l.Chunks.FirstOrDefault(o => o.Position == new Vector2(positionInChunk.X,                  positionInChunk.Y                 )));
                ChunksToDraw.Add(l.Chunks.FirstOrDefault(o => o.Position == new Vector2(positionInChunk.X + o.Dimensions.X, positionInChunk.Y + o.Dimensions.Y)));
                ChunksToDraw.Add(l.Chunks.FirstOrDefault(o => o.Position == new Vector2(positionInChunk.X - o.Dimensions.X, positionInChunk.Y - o.Dimensions.Y)));
                ChunksToDraw.Add(l.Chunks.FirstOrDefault(o => o.Position == new Vector2(positionInChunk.X + o.Dimensions.X, positionInChunk.Y - o.Dimensions.Y)));
                ChunksToDraw.Add(l.Chunks.FirstOrDefault(o => o.Position == new Vector2(positionInChunk.X - o.Dimensions.X, positionInChunk.Y + o.Dimensions.Y)));
                ChunksToDraw.Add(l.Chunks.FirstOrDefault(o => o.Position == new Vector2(positionInChunk.X + o.Dimensions.X, positionInChunk.Y                 )));
                ChunksToDraw.Add(l.Chunks.FirstOrDefault(o => o.Position == new Vector2(positionInChunk.X - o.Dimensions.X, positionInChunk.Y                 )));
                ChunksToDraw.Add(l.Chunks.FirstOrDefault(o => o.Position == new Vector2(positionInChunk.X,                  positionInChunk.Y + o.Dimensions.Y)));
                ChunksToDraw.Add(l.Chunks.FirstOrDefault(o => o.Position == new Vector2(positionInChunk.X,                  positionInChunk.Y - o.Dimensions.Y)));

                //draw chunks
                foreach(Chunk c in ChunksToDraw)
                {
                    for (int y = 0; y < c.Dimensions.Y; y++)
                    {
                        for (int x = 0; x < c.Dimensions.X; x++)
                        {
                            //check which tileset to use
                            foreach (int k in this.Tilesets.Keys)
                            {
                                //use the correct tileset
                                if(c.Data[x + y*(int)c.Dimensions.X] >= k && c.Data[x + y*(int)c.Dimensions.X] < (Tilesets[k].tilecount + k))
                                {
                                    Tileset tilesetToDraw = Tilesets[k];
                                    int LocalTileIndexToDraw = c.Data[x + y*(int)c.Dimensions.X] - k;

                                    //draw tile
                                    DrawTextureRec(tilesetToDraw.texture2D, tilesetToDraw.ReturnRect(LocalTileIndexToDraw), new Vector2((c.Position.X+x)*TileWidth, (c.Position.Y+y)*TileHeight), Color.WHITE);

                                    //check is tileproperties is empty
                                    if(tilesetToDraw.tileProperties.Count <= 0)
                                        continue;

                                    List<Tileset.TileProperties> tileProperties = tilesetToDraw.tileProperties[LocalTileIndexToDraw]; 
                                    foreach(var props in tileProperties)
                                    {
                                        CreateRpgEngine.manageProperties(l, c, y, x, tilesetToDraw, props, layers);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }



        public void Update()
        {
            Object.OnRemoveProperty += (string propertyName, int tilesetToDrawIndex, int tileIndex) =>
            {
                // get the properies of the tile and change it
                Tileset tileset = Tilesets[tilesetToDrawIndex];
                List<Tileset.TileProperties> tilePropertiesList = tileset.tileProperties[tileIndex];
                Tileset.TileProperties tileProperties = tilePropertiesList.FirstOrDefault(c => c.name == propertyName);
                int indexOfTileProperties = tilePropertiesList.IndexOf(tileProperties);

                //chage the property value
                tileProperties.value = false;

                // save the changed properties
                tilePropertiesList[indexOfTileProperties] = tileProperties;
                tileset.tileProperties[tileIndex] = tilePropertiesList;
                Tilesets[tilesetToDrawIndex] = tileset;

            };

            Object.OnRemoveTile += (int indexOfLayer, int indexOfChunk, int indexInChunk) => 
            {
                layers[indexOfLayer].Chunks[indexOfChunk].Data[indexInChunk] = 0;
            };
        }

        public struct Chunk 
        {
            public List<int> Data;
            public Vector2 Position;
            public Vector2 Dimensions;

            public Chunk(List<int> data, Vector2 position, Vector2 dimensions)
            {
                this.Data = data;
                this.Position = position;
                this.Dimensions = dimensions;
            }
        }

        public struct Layer
        {
            public List<Chunk> Chunks;
            public int Height;
            public int Width; 

            public Layer(List<Chunk> chunks, int height, int width)
            {
                this.Chunks = chunks;
                this.Height = height;
                this.Width = width;
            }
        }
    }
}