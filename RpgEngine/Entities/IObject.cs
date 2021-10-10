using System.Collections.Generic;
using RpgEngine.Renderer;

namespace RpgEngine.Entities
{
    public abstract class IObject 
    {
        public bool ShouldRemove = true;
        public abstract void Draw();
        public abstract void Update(ref Tilemap tilemap);
        public abstract void Unload();
    }
}