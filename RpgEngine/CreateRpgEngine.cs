using System;
using System.Collections.Generic;
using System.Numerics;
using static RpgEngine.Renderer.Tilemap;
using RpgEngine.Renderer;

namespace RpgEngine
{
    public class CreateRpgEngine
    {
        public static int ScreenHeight { get; set; }
        public static int ScreenWidth  { get; set; }
        public static ManageTileProperties manageTileProperties { get; set; }

        public CreateRpgEngine(int screenheight, int screenwidth)
        {
            ScreenHeight = screenheight;
            ScreenWidth = screenwidth;
        }

    }
    public delegate void ManageTileProperties(Layer l, Chunk c, int y, int x, Tileset tilesetToDraw, Tileset.TileProperties props, List<Layer> layers);
}
