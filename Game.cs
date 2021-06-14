using System;
using System.Numerics;
using System.Collections.Generic;
using RpgEngine;
using Raylib_cs;
using static RpgGameRaylib.PlayerStates;
using static Raylib_cs.Raylib;
using static RpgEngine.Tilemap;

namespace RpgGameRaylib
{
    class Game : CreateRpgEngine
    {
        public Game(int screenheight, int screenwidth)
            :base(screenheight, screenwidth) => Start();

        // Global Variables
        //--------------------------------------------------------------------------------------------------------
        public static int FPS = 60;
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

            // initialize player info
            PlayerStates.Init();
            PlayerAnimations.Init(henry);

            // manage the properties of the tiles
            manageProperties = new ManageProperties((Layer l, Chunk c, int y, int x, Tileset tilesetToDraw, Tileset.TileProperties props, List<Layer> layers) => 
            {
                if (props.name == "collide" && props.type == "bool")
                    if ((bool)props.value == true) CollisionManager.collisionBoxes.Add(
                        new CollisionBox(new Rectangle((c.Position.X + x) * tilesetToDraw.tilewidth, (c.Position.Y + y) * tilesetToDraw.tileheight, tilesetToDraw.tilewidth, tilesetToDraw.tileheight), true)
                    );

                if (props.name == Grass.Name && props.type == "bool")
                    if ((bool)props.value == true) ObjectManager.ObjectsList.Add(
                        new Grass(new CollisionBox(new Rectangle((c.Position.X + x) * tilesetToDraw.tilewidth, (c.Position.Y + y) * tilesetToDraw.tileheight, tilesetToDraw.tilewidth, tilesetToDraw.tileheight), true), layers.IndexOf(l), l.Chunks.IndexOf(c), (x + y * (int)c.Dimensions.X))
                    );
            });

            // update the collison box of the player
            updatePlayerCollisionBox = new UpdatePlayerCollisionBox((ref CollisionBox collisionBox, Vector2 position) => 
            {
                collisionBox.CollisionRect.x = position.X + 27;
                collisionBox.CollisionRect.y = position.Y + 37;
            });
            //--------------------------------------------------------------------------------------------------------

            // Game Loop
            while (!WindowShouldClose())
            {
                // Update
                //--------------------------------------------------------------------------------------------------------            
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

                // Update game
                henry.UpdateCollisionBox();
                henry.UpdateCamera();
                henry.animation.Update();
                /*temp*/CollisionManager.hitBoxes.Add(henry.collisionBox);
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
    }
}