using System;
using System.Timers;
using Raylib_cs;
using RpgEngine;
using static Raylib_cs.Raylib;

namespace RpgGameRaylib
{
    static class PlayerStates
    {
        public static Timer rollTimer = new Timer(2000);
        public static bool rollTimerReady = false;

        public static void Init()
        {
            // Set RollTimer
            rollTimer.Elapsed += (object source, ElapsedEventArgs e) =>
            {
                rollTimerReady = true;
            };
            rollTimer.AutoReset = true;
            rollTimer.Enabled = true;
        }

        public static void Update() { }
        public static void MoveState(Player player)
        {
            //function to change the state
            Func<bool> ChangeToRollState = () =>
            {
                if(rollTimerReady == true)
                    player.state = State.ROLL;
                rollTimerReady = false;
                return false;
            };
            
            // Manage Input
            if (IsKeyDown(player.input.Up))     { if (!CollisionManager.CheckCollisionWithAllObjects(new CollisionBox(new Rectangle(player.collisionBox.CollisionRect.x, player.collisionBox.CollisionRect.y - (player.Speed * GetFrameTime()), player.collisionBox.CollisionRect.width, player.collisionBox.CollisionRect.height), true))) player.position.Y -= (player.Speed * GetFrameTime()); player.UpdateCamera(); player.isMoving = true; player.direction = Directions.UP;    }
            if (IsKeyDown(player.input.Down))   { if (!CollisionManager.CheckCollisionWithAllObjects(new CollisionBox(new Rectangle(player.collisionBox.CollisionRect.x, player.collisionBox.CollisionRect.y + (player.Speed * GetFrameTime()), player.collisionBox.CollisionRect.width, player.collisionBox.CollisionRect.height), true))) player.position.Y += (player.Speed * GetFrameTime()); player.UpdateCamera(); player.isMoving = true; player.direction = Directions.DOWN;  }
            if (IsKeyDown(player.input.Left))   { if (!CollisionManager.CheckCollisionWithAllObjects(new CollisionBox(new Rectangle(player.collisionBox.CollisionRect.x - (player.Speed * GetFrameTime()), player.collisionBox.CollisionRect.y, player.collisionBox.CollisionRect.width, player.collisionBox.CollisionRect.height), true))) player.position.X -= (player.Speed * GetFrameTime()); player.UpdateCamera(); player.isMoving = true; player.direction = Directions.LEFT;  }
            if (IsKeyDown(player.input.Right))  { if (!CollisionManager.CheckCollisionWithAllObjects(new CollisionBox(new Rectangle(player.collisionBox.CollisionRect.x + (player.Speed * GetFrameTime()), player.collisionBox.CollisionRect.y, player.collisionBox.CollisionRect.width, player.collisionBox.CollisionRect.height), true))) player.position.X += (player.Speed * GetFrameTime()); player.UpdateCamera(); player.isMoving = true; player.direction = Directions.RIGHT; }
            if (IsKeyPressed(player.input.Attack)) { player.state = State.ATTACK; }
            if (IsKeyPressed(player.input.Roll))
            { 
                switch(player.direction)
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
            switch (player.direction)
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

        public static void AttackState(Player player)
        {
            switch (player.direction)
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

        public static void RollState(Player player)
        {
            switch(player.direction)
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

    public static class PlayerAnimations
    {
        public static void Init(Player player)
        {
            // Create Animations
            player.animation.CreateAnimation(new int[] { 6  }, "IdleUp"   , 8);
            player.animation.CreateAnimation(new int[] { 12 }, "IdleLeft" , 8);
            player.animation.CreateAnimation(new int[] { 18 }, "IdleDown" , 8);
            player.animation.CreateAnimation(new int[] { 0  }, "IdleRight", 8);

            player.animation.CreateAnimation(new int[] { 7,  8,  9,  10, 11, 6  }, "RunUp"   , 8);
            player.animation.CreateAnimation(new int[] { 13, 14, 15, 16, 17, 12 }, "RunLeft" , 8);
            player.animation.CreateAnimation(new int[] { 19, 20, 21, 22, 23, 18 }, "RunDown" , 8);
            player.animation.CreateAnimation(new int[] { 1,  2,  3,  4,  5,  0  }, "RunRight", 8);

            player.animation.CreateAnimation(new int[] { 28, 29, 30, 31 }, "AttackUp"   , 8);
            player.animation.CreateAnimation(new int[] { 32, 33, 34, 35 }, "AttackLeft" , 8);
            player.animation.CreateAnimation(new int[] { 36, 37, 38, 39 }, "AttackDown" , 8);
            player.animation.CreateAnimation(new int[] { 24, 25, 26, 27 }, "AttackRight", 8);

            player.animation.CreateAnimation(new int[] { 45, 46, 47, 48, 49 }, "RollUp"   , 8);
            player.animation.CreateAnimation(new int[] { 50, 51, 52, 53, 54 }, "RollLeft" , 8);
            player.animation.CreateAnimation(new int[] { 55, 56, 57, 58, 59 }, "RollDown" , 8);
            player.animation.CreateAnimation(new int[] { 40, 41, 42, 43, 44 }, "RollRight", 8);

            //Change state when attack and roll animations are finished
            player.animation.OnAnimationFinished += (string animationName) => 
            {
                if(animationName == "AttackUp" || animationName == "AttackLeft" || 
                   animationName == "AttackDown" || animationName == "AttackRight")
                player.state = State.MOVE;

                if(animationName == "RollUp" || animationName == "RollLeft" || 
                   animationName == "RollDown" || animationName == "RollRight")
                player.state = State.MOVE;
            };
        }
    }
}