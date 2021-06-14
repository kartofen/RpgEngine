using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace RpgEngine
{
    public class Player : Sprite
    {
        public State state;
        public Directions direction;
        public Camera2D camera;
        public Vector2 position;
        public Vector2 center;
        public Input input;
        public CollisionBox collisionBox;
        public AnimationManager animation;
        public float Speed;
        public bool isMoving = false;
        public Vector2 FrameDimensions;

        public Player(Texture2D texture, Vector2 pos, Vector2 FrameDimensions, float speed = 1, float CameraZoom = 1, Directions startingDirection = Directions.LEFT)
            :base(texture)
        {
            this.position = pos;
            this.direction = startingDirection;
            this.Speed = speed;
            this.center = new Vector2(this.texture2D.width/FrameDimensions.X/2, this.texture2D.height/FrameDimensions.Y/2);
            this.camera = new Camera2D(new Vector2(CreateRpgEngine.ScreenWidth/2, CreateRpgEngine.ScreenHeight/2), new Vector2(this.position.X + this.center.X, this.position.Y + this.center.Y), 0, CameraZoom);
            this.FrameDimensions = FrameDimensions;
            this.state = State.MOVE;
        }

        public void UpdateCamera()
        {
            //Update camera to change postition to the player
            this.camera.target = new Vector2(position.X + center.X, position.Y + center.Y);
        }
        
        public void UpdateCollisionBox()
        {
            // this.collisionBox.CollisionRect.x = position.X + texture2D.width/FrameDimensions.X/2 - collisionBox.CollisionRect.width/2;
            // this.collisionBox.CollisionRect.y = position.Y + ((this.texture2D.height/FrameDimensions.Y/3) * 1.8f);
            CreateRpgEngine.updatePlayerCollisionBox(ref collisionBox, position);
        }

        public void Draw()
        {
            DrawTextureRec(this.texture2D, this.animation.FrameRect, this.position, Color.WHITE);
        }
    }
}
