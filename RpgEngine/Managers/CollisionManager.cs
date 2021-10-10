using System.Collections.Generic;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace RpgEngine.Managers
{
    public static class CollisionManager
    {
        public static List<CollisionBox> collisionBoxes = new List<CollisionBox>();
        public static List<CollisionBox> hitBoxes = new List<CollisionBox>();

        public static bool CheckCollisionWithAllObjects(CollisionBox boxToCheck)
        {
            foreach (var box in collisionBoxes)
            {
                if(box.Enabled && boxToCheck.Enabled)
                    if(CheckCollisionRecs(boxToCheck.CollisionRect, box.CollisionRect)) return true;
                    else continue;
            }
            return false;  
        }

        public static bool ChechCollisionWithHitBoxes(CollisionBox boxToCheck)
        {
            foreach (var box in hitBoxes)
            {
                if(box.Enabled && boxToCheck.Enabled)
                    if(CheckCollisionRecs(boxToCheck.CollisionRect, box.CollisionRect)) return true;
                    else continue;
            }
            return false;  
        }
    }

    public struct CollisionBox
    {
        public Rectangle CollisionRect;
        public bool Enabled;
        public CollisionBox(Rectangle collisisonRect, bool enabled)
        {
            CollisionRect = collisisonRect;
            Enabled = enabled;
        }
    }
}