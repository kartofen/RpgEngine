using System.Numerics;
using RpgEngine.Renderer;

namespace RpgEngine.Entities
{
    public abstract class IObject 
    {
        public static string Name;
        public Vector2 position;
        public bool ShouldRemove = true;
        public abstract void Draw();
        public abstract void Update(ref Tilemap tilemap);
        public abstract void Unload();
    }
}