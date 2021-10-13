using System.Collections.Generic;
using System.Numerics;
using RpgEngine.Entities;
using RpgEngine.Managers;
using RpgEngine.Renderer;
using RpgEngine.Sprites;
using Raylib_cs;
using static Raylib_cs.Raylib;
using System;

namespace RpgGameRaylib.Entities
{
    public class Bush : IObject
    {
        private readonly int IndexOfLayer;
        private readonly int IndexOfChunk;
        private readonly int IndexInChunk;      

        public Bush(int indexOfLayer, int indexOfChunk, int indexInChunk)
        {
            Name = "bush";
            IndexOfLayer = indexOfLayer;
            IndexOfChunk = indexOfChunk;
            IndexInChunk = indexInChunk;
        }
        
        public override void Update(ref Tilemap tilemap)
        {
            
        }

        public override void Unload()
        {

        }

        public override void Draw()
        {  

        }
    }
}