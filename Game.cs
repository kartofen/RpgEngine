using System;
using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;
using static Raylib_cs.Raylib;
using RpgEngine;
using RpgEngine.Renderer;
using RpgEngine.Componets;
using RpgEngine.Managers;
using RpgGameRaylib.Components;
using RpgGameRaylib.Entities;

/**
 * TODO: draw the entities and objects when it draws the tiles, not over all of them
 *       change manageTileProperties so its easier to write and read the code when adding an entity
 *       add entity loading from the tilemap with the entitylayer in the tilemap
 *       ? optimize the game
 */

namespace RpgGameRaylib
{
    class Game : CreateRpgEngine
    {
        public Game(int screenheight, int screenwidth)
            :base(screenheight, screenwidth) => Start();

        public static void Start()
        {
            // Initialize Window and Engine
            InitWindow(ScreenWidth, ScreenHeight, "Rpg Game");
            SetTargetFPS(60); //FPS is 60
            SetExitKey(KeyboardKey.KEY_NULL); // Set no Exit Key

            // Initialize
            //------------------------------------------------------------------------------------------------------------
            //PLAYER
            Player henry = new Player(LoadTexture("Assets/Player/Player1.png"), new Vector2(0), new Vector2(60, 1), 100f, 2f)
            {
                collisionBox = new CollisionBox(new Rectangle(0, 0, 10, 7), true),
                input = new Input() 
                { 
                    Up = KeyboardKey.KEY_W,
                    Left = KeyboardKey.KEY_A, 
                    Down = KeyboardKey.KEY_S, 
                    Right = KeyboardKey.KEY_D, 
                    Attack = KeyboardKey.KEY_SPACE, 
                    Roll = KeyboardKey.KEY_LEFT_SHIFT 
                }
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
                henry.psm.state = State.MOVE;

                if(animationName == "RollUp" || animationName == "RollLeft" || 
                   animationName == "RollDown" || animationName == "RollRight")
                henry.psm.state = State.MOVE;
            };
            // add the player as an entity
            EntityManager.Entities.Add(henry);

            // TILEMAP
            Tilemap tilemap = Tilemap.LoadTilemap("Assets/World1.tmx");
            // manage the properties of the tiles
            tilemap.manageTileProperties = new Tilemap.ManageTileProperties((Tilemap.Layer l, Tilemap.Chunk c, int y, int x, Tileset tilesetToDraw, Tileset.TileProperties props, List<Tilemap.Layer> layers) => 
            {
                if (props.name == "collide" && props.type == "bool")
                    if ((bool)props.value == true) ObjectManager.Objects.Add(
                        new Cliff(layers.IndexOf(l), l.Chunks.IndexOf(c), (x + y * (int)c.Dimensions.X), new CollisionBox(new Rectangle((c.Position.X + x) * tilesetToDraw.tilewidth, (c.Position.Y + y) * tilesetToDraw.tileheight, tilesetToDraw.tilewidth, tilesetToDraw.tileheight), true))
                    );

                if (props.name == "grass" && props.type == "bool")
                    if ((bool)props.value == true) ObjectManager.Objects.Add(
                        new Grass(new CollisionBox(new Rectangle((c.Position.X + x) * tilesetToDraw.tilewidth, (c.Position.Y + y) * tilesetToDraw.tileheight, tilesetToDraw.tilewidth, tilesetToDraw.tileheight), true), layers.IndexOf(l), l.Chunks.IndexOf(c), (x + y * (int)c.Dimensions.X))
                    );

                //if (props.name == "bush" && props.type == "bool")
                //    if ((bool)props.value == true) ObjectManager.Objects.Add(
                //        new Bush(layers.IndexOf(l), l.Chunks.IndexOf(c), (x + y * (int)c.Dimensions.X))
                //    );
            });
            //------------------------------------------------------------------------------------------------------------

            // Game Loop
            while (!WindowShouldClose())
            {
                // Update
                //--------------------------------------------------------------------------------------------------------            
                // Update game
                EntityManager.Update();
                /*temp*/CollisionManager.hitBoxes.Add(henry.collisionBox);
                ObjectManager.Update(ref tilemap);
                //--------------------------------------------------------------------------------------------------------

                // Draw
                //--------------------------------------------------------------------------------------------------------
                BeginDrawing();

                    ClearBackground(Color.BLACK);

                        BeginMode2D(henry.camera);

                            // the tilemap draw the objects and entitites too
                            tilemap.Draw(new Vector2(henry.center.X + henry.position.X, henry.center.Y + henry.position.Y));

                            //DrawRectangle((int)henry.position.X, (int)henry.position.Y, 1, 1, Color.MAGENTA);

                            //draw hitbox
                            //DrawRectangle((int)henry.collisionBox.CollisionRect.x, (int)henry.collisionBox.CollisionRect.y, (int)henry.collisionBox.CollisionRect.width, (int)henry.collisionBox.CollisionRect.height, Color.MAGENTA);

                            ObjectManager.Draw();

                            EntityManager.Draw();

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
            //------------------------------------------------------------------------------------------------------------
            EntityManager.Unload();

            ObjectManager.Unload();

            tilemap.UnloadTilemap();

            CloseWindow();
            //------------------------------------------------------------------------------------------------------------
        }
    }
}