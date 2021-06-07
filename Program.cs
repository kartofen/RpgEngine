using System;
using System.Numerics;
using System.Collections.Generic;
using RpgEngine;
using Raylib_cs;
using static Raylib_cs.Raylib;
using static RpgEngine.Tilemap;

namespace RpgGameRaylib
{    
    static class Program
    {       
        public static int ScreenHeight = 600;
        public static int ScreenWidth = 970;

        public static void Main()
        {
            new Game(ScreenHeight, ScreenWidth);
        }
    }

    class Game : CreateRpgEngine
    {
        public Game(int screenheight, int screenwidth)
            :base(screenheight, screenwidth) => Start();

        // Global Variables
        //--------------------------------------------------------------------------------------------------------
        public static int FPS = 60;

        public static float rollTimer = 0;
        public static float rollTimerMax = 2.9f;
        //--------------------------------------------------------------------------------------------------------

        public void Start()
        {
            // Initialize Window and Engine
            InitWindow(ScreenWidth, ScreenHeight, "Rpg Game");
            SetTargetFPS(FPS);

            // Initialize
            //--------------------------------------------------------------------------------------------------------
            Tilemap tilemap = Tilemap.LoadTilemap("Assets/World1.tmx");
            Player henry = new Player(LoadTexture("Assets/Player/Player1.png"), new Vector2(0), new Vector2(60, 1), 100f, 2f);
            henry.collisionBox = new CollisionBox(new Rectangle(0, 0, 10, 7), true);
            henry.animation = new AnimationManager(henry.texture2D, new Vector2(60, 1));
            henry.input = new Input() 
            { 
                Up = KeyboardKey.KEY_W,
                Left = KeyboardKey.KEY_A, 
                Down = KeyboardKey.KEY_S, 
                Right = KeyboardKey.KEY_D, 
                Attack = KeyboardKey.KEY_SPACE, 
                Roll = KeyboardKey.KEY_LEFT_SHIFT 
            };

            // Create Animations
            henry.animation.CreateAnimation(new int[] { 6  }, "IdleUp"   , 8);
            henry.animation.CreateAnimation(new int[] { 12 }, "IdleLeft" , 8);
            henry.animation.CreateAnimation(new int[] { 18 }, "IdleDown" , 8);
            henry.animation.CreateAnimation(new int[] { 0  }, "IdleRight", 8);

            henry.animation.CreateAnimation(new int[] { 7,  8,  9,  10, 11, 6  }, "RunUp"   , 8);
            henry.animation.CreateAnimation(new int[] { 13, 14, 15, 16, 17, 12 }, "RunLeft" , 8);
            henry.animation.CreateAnimation(new int[] { 19, 20, 21, 22, 23, 18 }, "RunDown" , 8);
            henry.animation.CreateAnimation(new int[] { 1,  2,  3,  4,  5,  0  }, "RunRight", 8);

            henry.animation.CreateAnimation(new int[] { 28, 29, 30, 31 }, "AttackUp"   , 8);
            henry.animation.CreateAnimation(new int[] { 32, 33, 34, 35 }, "AttackLeft" , 8);
            henry.animation.CreateAnimation(new int[] { 36, 37, 38, 39 }, "AttackDown" , 8);
            henry.animation.CreateAnimation(new int[] { 24, 25, 26, 27 }, "AttackRight", 8);

            henry.animation.CreateAnimation(new int[] { 45, 46, 47, 48, 49 }, "RollUp"   , 8);
            henry.animation.CreateAnimation(new int[] { 50, 51, 52, 53, 54 }, "RollLeft" , 8);
            henry.animation.CreateAnimation(new int[] { 55, 56, 57, 58, 59 }, "RollDown" , 8);
            henry.animation.CreateAnimation(new int[] { 40, 41, 42, 43, 44 }, "RollRight", 8);

            //Change state when attack and roll animations are finished
            henry.animation.OnAnimationFinished += (string animationName) => 
            {
                if(animationName == "AttackUp" || animationName == "AttackLeft" || 
                   animationName == "AttackDown" || animationName == "AttackRight")
                henry.state = State.MOVE;

                if(animationName == "RollUp" || animationName == "RollLeft" || 
                   animationName == "RollDown" || animationName == "RollRight")
                henry.state = State.MOVE;
            };

            manageProperties = new ManageProperties(_ManageProperties);
            updatePlayerCollisionBox = new UpdatePlayerCollisionBox(_UpdatePlayerCollisionBox);
            //--------------------------------------------------------------------------------------------------------

            // Game Loop
            while (!WindowShouldClose())
            {
                // Update
                //--------------------------------------------------------------------------------------------------------            
                //Update things
                henry.UpdateCollisionBox();
                henry.UpdateCamera();
                henry.animation.Update();
                /*temp*/CollisionManager.hitBoxes.Add(henry.collisionBox);

                //Update the roll timer
                if(rollTimer < rollTimerMax)
                    rollTimer += 1 * GetFrameTime();

                switch (henry.state)
                {
                    case State.MOVE:
                        MoveState(henry);
                    break;
                    case State.ATTACK:
                        AttackState(henry);
                    break;
                    case State.ROLL:
                        RollState(henry);
                    break;
                }

                ObjectManager.Update();
                tilemap.Update();
                //--------------------------------------------------------------------------------------------------------

                // Draw
                //--------------------------------------------------------------------------------------------------------
                BeginDrawing();

                    ClearBackground(Color.BLACK);

                        BeginMode2D(henry.camera);

                            tilemap.Draw(new Vector2(henry.center.X + henry.position.X, henry.center.Y + henry.position.Y));

                            //draw hitbox
                            DrawRectangle((int)henry.collisionBox.CollisionRect.x, (int)henry.collisionBox.CollisionRect.y, (int)henry.collisionBox.CollisionRect.width, (int)henry.collisionBox.CollisionRect.height, Color.MAGENTA);

                            henry.Draw();

                            //draw collision boxes
                            // foreach (var i in CollisionManager.collisionBoxes)
                            //    DrawRectangle((int)i.CollisionRect.x, (int)i.CollisionRect.y, (int)i.CollisionRect.width, (int)i.CollisionRect.height, Color.WHITE);  

           
                        EndMode2D();
                    
                    //DrawText(rollTimer+"", 10, 50, 5, Color.MAGENTA);

                    //DrawText(henry.collisionBox.CollisionRect.x +" "+ henry.collisionBox.CollisionRect.y, 10, 60, 5, Color.MAGENTA);

                    DrawFPS(10, 10);

                EndDrawing();
                //--------------------------------------------------------------------------------------------------------
            }

            // De-Initialize
            //--------------------------------------------------------------------------------------------------------
            henry.UnloadSprite();

            tilemap.UnloadTilemap();

            CloseWindow();
            //--------------------------------------------------------------------------------------------------------
        }
        private static void MoveState(Player player)
        {
            //function to change the state
            Func<bool> ChangeToRollState = () =>
            {
                if(rollTimer >= rollTimerMax)
                {
                    rollTimer = 0f;
                    player.state = State.ROLL;
                }
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

        private static void AttackState(Player player)
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

        private static void RollState(Player player)
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

        private void _ManageProperties(Layer l, Chunk c, int y, int x, Tileset tilesetToDraw, Tileset.TileProperties props, List<Layer> layers)
        {
            if (props.name == "collide" && props.type == "bool")
                if ((bool)props.value == true) CollisionManager.collisionBoxes.Add(
                    new CollisionBox(new Rectangle((c.Position.X + x) * tilesetToDraw.tilewidth, (c.Position.Y + y) * tilesetToDraw.tileheight, tilesetToDraw.tilewidth, tilesetToDraw.tileheight), true)
                );

            if (props.name == Grass.Name && props.type == "bool")
                if ((bool)props.value == true) ObjectManager.ObjectsList.Add(
                    new Grass(new CollisionBox(new Rectangle((c.Position.X + x) * tilesetToDraw.tilewidth, (c.Position.Y + y) * tilesetToDraw.tileheight, tilesetToDraw.tilewidth, tilesetToDraw.tileheight), true), layers.IndexOf(l), l.Chunks.IndexOf(c), (x + y * (int)c.Dimensions.X))
                    );
        }

        private void _UpdatePlayerCollisionBox(ref CollisionBox collisionBox, Vector2 position)
        {
            collisionBox.CollisionRect.x = position.X + 27;
            collisionBox.CollisionRect.y = position.Y + 37;
        }
    }
}
