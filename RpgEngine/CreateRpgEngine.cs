using System;
using System.Collections.Generic;
using System.Numerics;
using static RpgEngine.Tilemap;

namespace RpgEngine
{
    public class CreateRpgEngine
    {
        public static int ScreenHeight { get; set; }
        public static int ScreenWidth  { get; set; }
        public static ManageProperties manageProperties { get; set; }
        public static UpdatePlayerCollisionBox updatePlayerCollisionBox { get; set; }

        public CreateRpgEngine(int screenheight, int screenwidth)
        {
            ScreenHeight = screenheight;
            ScreenWidth = screenwidth;
        }

    }
    public delegate void ManageProperties(Layer l, Chunk c, int y, int x, Tileset tilesetToDraw, Tileset.TileProperties props, List<Layer> layers);
    public delegate void UpdatePlayerCollisionBox(ref CollisionBox collisionBox, Vector2 position);
}
