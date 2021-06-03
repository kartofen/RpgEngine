using System.Collections.Generic;
using RpgEngine;

namespace RpgGameRaylib
{
    public class Grass : Object
    {
        public static string Name = "grass";
        public CollisionBox HurtBox;

        private int IndexOfLayer;
        private int IndexOfChunk;
        private int IndexInChunk;      

        public Grass(CollisionBox collisionBox, int indexOfLayer, int indexOfChunk, int indexInChunk)
        {
            HurtBox = collisionBox;
            IndexOfLayer = indexOfLayer;
            IndexOfChunk = indexOfChunk;
            IndexInChunk = indexInChunk;
        }
        
        public override void Update()
        {
            if(CollisionManager.ChechCollisionWithHitBoxes(HurtBox))
                StartRemoveTileEvent(IndexOfLayer, IndexOfChunk, IndexInChunk);
        }
    }
}