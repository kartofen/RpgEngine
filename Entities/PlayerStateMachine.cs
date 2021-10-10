using System.Timers;
using Raylib_cs;
using RpgEngine.Managers;
using static Raylib_cs.Raylib;
using RpgGameRaylib.Components;

namespace RpgGameRaylib.Entities
{
    public class PlayerStateMachine
    {
        public State state { get; set; }
        public Directions direction { get; set; }
        private static readonly Timer rollTimer = new Timer(2000);
        private static bool rollTimerReady = false;
        private static Player player;

        public PlayerStateMachine(Player p)
        {
            // Set RollTimer
            rollTimer.Elapsed += (object source, ElapsedEventArgs e) => rollTimerReady = true;
            rollTimer.AutoReset = true;
            rollTimer.Enabled = true;
            player = p;
        }

        public void Update() 
        { 
            switch (state)
            {
                case State.MOVE:
                    MoveState();
                break;
                case State.ATTACK:
                    AttackState();
                break;
                case State.ROLL:
                    RollState();
                break;
            }
        }

        public void MoveState()
        {
            //function to change the state
            bool ChangeToRollState()
            {
                if (rollTimerReady == true)
                    state = State.ROLL;
                rollTimerReady = false;
                return false;
            }

            // Manage Input
            if (IsKeyDown(player.input.Up))     { if (!CollisionManager.CheckCollisionWithAllObjects(new CollisionBox(new Rectangle(player.collisionBox.CollisionRect.x, player.collisionBox.CollisionRect.y - (player.Speed * GetFrameTime()), player.collisionBox.CollisionRect.width, player.collisionBox.CollisionRect.height), true))) player.position.Y -= (player.Speed * GetFrameTime()); player.isMoving = true; direction = Directions.UP;    }
            if (IsKeyDown(player.input.Down))   { if (!CollisionManager.CheckCollisionWithAllObjects(new CollisionBox(new Rectangle(player.collisionBox.CollisionRect.x, player.collisionBox.CollisionRect.y + (player.Speed * GetFrameTime()), player.collisionBox.CollisionRect.width, player.collisionBox.CollisionRect.height), true))) player.position.Y += (player.Speed * GetFrameTime()); player.isMoving = true; direction = Directions.DOWN;  }
            if (IsKeyDown(player.input.Left))   { if (!CollisionManager.CheckCollisionWithAllObjects(new CollisionBox(new Rectangle(player.collisionBox.CollisionRect.x - (player.Speed * GetFrameTime()), player.collisionBox.CollisionRect.y, player.collisionBox.CollisionRect.width, player.collisionBox.CollisionRect.height), true))) player.position.X -= (player.Speed * GetFrameTime()); player.isMoving = true; direction = Directions.LEFT;  }
            if (IsKeyDown(player.input.Right))  { if (!CollisionManager.CheckCollisionWithAllObjects(new CollisionBox(new Rectangle(player.collisionBox.CollisionRect.x + (player.Speed * GetFrameTime()), player.collisionBox.CollisionRect.y, player.collisionBox.CollisionRect.width, player.collisionBox.CollisionRect.height), true))) player.position.X += (player.Speed * GetFrameTime()); player.isMoving = true; direction = Directions.RIGHT; }
            if (IsKeyPressed(player.input.Attack)) { state = State.ATTACK; }
            if (IsKeyPressed(player.input.Roll))
            { 
                switch(direction)
                {
                    case Directions.UP:
                        //41 is how many pixels the character moves when it roll                        
                        player.collisionBox.CollisionRect.y -= 41;
                        if(CollisionManager.CheckCollisionWithAllObjects(player.collisionBox))
                            player.collisionBox.CollisionRect.y += 41;
                        else
                            ChangeToRollState();
                        break;
                    case Directions.LEFT:
                        //41 is how many pixels the character moves when it roll                        
                        player.collisionBox.CollisionRect.x -= 41;
                        if(CollisionManager.CheckCollisionWithAllObjects(player.collisionBox))
                            player.collisionBox.CollisionRect.x += 41;
                        else
                            ChangeToRollState();
                        break;
                    case Directions.DOWN:
                        //41 is how many pixels the character moves when it roll
                        player.collisionBox.CollisionRect.y += 41;
                        if(CollisionManager.CheckCollisionWithAllObjects(player.collisionBox))
                            player.collisionBox.CollisionRect.y -= 41;
                        else
                            ChangeToRollState();
                        break;
                    case Directions.RIGHT:
                        //41 is how many pixels the character moves when it roll
                        player.collisionBox.CollisionRect.x += 41;
                        if(CollisionManager.CheckCollisionWithAllObjects(player.collisionBox))
                            player.collisionBox.CollisionRect.x -= 41;
                        else
                            ChangeToRollState();
                        break;
                }
            }
            //zoom in and out the camera
            player.camera.zoom += GetMouseWheelMove() * 0.3f; // 0.3f is a random constant

            // Handle Animations
            switch (direction)
            {
                case Directions.UP:
                    if (!player.isMoving) player.animation.PlayAnimation("IdleUp");
                    else player.animation.PlayAnimation("RunUp");
                    break;
                case Directions.LEFT:
                    if (!player.isMoving) player.animation.PlayAnimation("IdleLeft");
                    else player.animation.PlayAnimation("RunLeft");
                    break;
                case Directions.DOWN:
                    if (!player.isMoving) player.animation.PlayAnimation("IdleDown");
                    else player.animation.PlayAnimation("RunDown");
                    break;
                case Directions.RIGHT:
                    if (!player.isMoving) player.animation.PlayAnimation("IdleRight");
                    else player.animation.PlayAnimation("RunRight");
                    break;
            }

            player.isMoving = false;
        }

        public void AttackState()
        {
            switch (direction)
            {
                case Directions.UP:
                    player.animation.PlayAnimation("AttackUp");
                    break;
                case Directions.LEFT:
                    player.animation.PlayAnimation("AttackLeft");
                    break;
                case Directions.DOWN:
                    player.animation.PlayAnimation("AttackDown");
                    break;
                case Directions.RIGHT:
                    player.animation.PlayAnimation("AttackRight");
                    break;
            }
        }

        public void RollState()
        {
            switch(direction)
            {
                case Directions.UP:
                    player.animation.PlayAnimation("RollUp");
                    player.position.Y -= (player.Speed-10) * GetFrameTime();
                    break;
                case Directions.LEFT:
                    player.animation.PlayAnimation("RollLeft");
                    player.position.X -= (player.Speed-10) * GetFrameTime();
                    break;
                case Directions.DOWN:
                    player.animation.PlayAnimation("RollDown");
                    player.position.Y += (player.Speed-10) * GetFrameTime();
                    break;
                case Directions.RIGHT:
                    player.animation.PlayAnimation("RollRight");
                    player.position.X += (player.Speed-10) * GetFrameTime();
                    break;
            }
        }
    }
}
