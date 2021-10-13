using System.Numerics;
using RpgEngine.Sprites;
using RpgEngine.Managers;

namespace RpgEngine.Entities
{   
    public abstract class IEntity
    {
        public Sprite sprite;
        public Vector2 position;
        public Vector2 center;
        public Vector2 FrameDimensions;
        public CollisionBox collisionBox;

        public abstract void Update();
        public abstract void Unload();
        public abstract void Draw();
    }
}