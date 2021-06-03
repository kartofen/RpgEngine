using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace RpgEngine
{
    public class Sprite
    {
        public Texture2D texture2D;
        
        public Sprite(Texture2D texture)
        {
            this.texture2D = texture;
        }

        public void UnloadSprite()
        {
            UnloadTexture(this.texture2D);
        }
    }
}
