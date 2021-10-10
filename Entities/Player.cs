using System.Numerics;
using Raylib_cs;
using RpgEngine;
using RpgEngine.Sprites;
using RpgEngine.Componets;
using RpgEngine.Managers;
using RpgEngine.Entities;
using RpgGameRaylib;
using static Raylib_cs.Raylib;

namespace RpgGameRaylib.Entities
{
    public class Player : IEntity
    {
        public PlayerStateMachine psm;
        public Camera2D camera;
        public Input input;
        public AnimationManager animation;
        public float Speed;
        public bool isMoving = false;
        public Vector2 FrameDimensions;

        public Player(Texture2D texture, Vector2 pos, Vector2 FrameDimensions, float speed = 1, float CameraZoom = 1)
        {
            this.sprite = new Sprite(texture);
            this.position = pos;
            this.Speed = speed;
            this.center = new Vector2(this.sprite.texture2D.width/FrameDimensions.X/2, this.sprite.texture2D.height/FrameDimensions.Y/2);
            this.camera = new Camera2D(new Vector2(CreateRpgEngine.ScreenWidth/2, CreateRpgEngine.ScreenHeight/2), new Vector2(this.position.X + this.center.X, this.position.Y + this.center.Y), 0, CameraZoom);
            this.FrameDimensions = FrameDimensions;
            this.animation = new AnimationManager(this.sprite.texture2D, new Vector2(60, 1));
            psm = new PlayerStateMachine(this);
        }

        public override void Update()
        {
            // update collision box
            collisionBox.CollisionRect.x = position.X + 27;
            collisionBox.CollisionRect.y = position.Y + 37;
            
            //update player state machine
            psm.Update();

            // update animation manager
            animation.Update();

            // update camera
            this.camera.target = new Vector2(position.X + center.X, position.Y + center.Y);

        }

        public override void Unload()
        {
            this.sprite.UnloadSprite();
        }
        
        public override void Draw()
        {
            DrawTextureRec(this.sprite.texture2D, this.animation.FrameRect, this.position, Color.WHITE);
        }
    }
}