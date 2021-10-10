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
    public class Grass : IObject
    {
        public static readonly string Name = "grass";
        public CollisionBox HurtBox;
        private readonly int IndexOfLayer;
        private readonly int IndexOfChunk;
        private readonly int IndexInChunk;      
        private Sprite sprite;
        AnimationManager animation;
        private bool AnimationStarted = false;
        private Vector2 position; // in pixels

        public Grass(CollisionBox collisionBox, int indexOfLayer, int indexOfChunk, int indexInChunk)
        {
            HurtBox = collisionBox;
            IndexOfLayer = indexOfLayer;
            IndexOfChunk = indexOfChunk;
            IndexInChunk = indexInChunk;
        }
        
        public override void Update(ref Tilemap tilemap)
        {
            if(AnimationStarted)
            {
                animation.Update();
                animation.PlayAnimation("GrassEffect");
            }

            if(CollisionManager.ChechCollisionWithHitBoxes(HurtBox) && !AnimationStarted)
            {
                tilemap.RemoveTile(IndexOfLayer, IndexOfChunk, IndexInChunk);
                // get position of the tile
                position = tilemap.GetTilePosition(IndexOfLayer, IndexOfChunk, IndexInChunk);
                
                // create animation and sprite
                sprite = new Sprite(LoadTexture("Assets/Effects/GrassEffect.png"));
                animation = new AnimationManager(sprite.texture2D, new Vector2(5, 1));
                animation.CreateAnimation(new int[] {0, 1, 2, 3, 4}, "GrassEffect", 8);

                ShouldRemove = false;
                AnimationStarted = true;

                // unload the sprite and remove the object from memory when the animation finishes
                animation.OnAnimationFinished += (string animationName) =>
                {
                    sprite.UnloadSprite();
                    ShouldRemove = true;
                };
            }
        }

        public override void Unload()
        {
            //sprite.UnloadSprite();
        }

        public override void Draw()
        {  
            if(AnimationStarted)
                // add some offset to the postion because the animation frame sis not the size of a tile
                DrawTextureRec(this.sprite.texture2D, animation.FrameRect, new Vector2(position.X - 7, position.Y - 7), Color.WHITE);
        }
    }
}