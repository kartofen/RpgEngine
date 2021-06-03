using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using System.Linq;
using static Raylib_cs.Raylib;

namespace RpgEngine
{
    public class AnimationManager
    {
        public Rectangle FrameRect;
        public List<Animation> animations;
        //private int currentIndex;
        private int frameCounter;
        public int OneTextureWidth;
        public int OneTextureHeight;
        public Vector2 FrameDimentions;
        public delegate void AnimationFinished(string animationName);
        public event AnimationFinished OnAnimationFinished;

        public AnimationManager(Texture2D texture, Vector2 frameDimentions, int frameSpeed = 1)
        {
            this.animations = new List<Animation>();
            this.OneTextureWidth = texture.width/(int)frameDimentions.X;
            this.OneTextureHeight = texture.height/(int)frameDimentions.Y;
            this.FrameRect = new Rectangle(0, 0, OneTextureWidth, OneTextureHeight);
            this.FrameDimentions = frameDimentions;
        }

        public void CreateAnimation(int[] frames, string name, int frameSpeed)
        {
            this.animations.Add(new Animation(name, frames, frameSpeed));
        }

        public void PlayAnimation(string animationName)
        {
            Animation currentAnimation = animations.FirstOrDefault(c => c.Name == animationName);   
            int index = animations.IndexOf(currentAnimation);
            if(this.frameCounter >= (GetFPS()/currentAnimation.FrameSpeed))
            {
                this.frameCounter = 0;
                int AnimationIndex = currentAnimation.Frames[currentAnimation.currentIndex];
                Vector2 AnimationIndexCoords = new Vector2((int)(AnimationIndex % FrameDimentions.X), (int)(AnimationIndex / FrameDimentions.X));
                this.FrameRect.x = AnimationIndex * OneTextureWidth;
                this.FrameRect.y = AnimationIndexCoords.Y * OneTextureHeight;
                currentAnimation.currentIndex++;
                animations[index] = currentAnimation;
            }

            //call the event when the animation is finished
            if(currentAnimation.currentIndex >= currentAnimation.Frames.Length)
            {
                OnAnimationFinished(animationName);
                currentAnimation.currentIndex = 0;
                animations[index] = currentAnimation;
            }
        }

        public void Update()
        {
            //IncrementFrameCounter
            this.frameCounter++;
        }

        public struct Animation
        {
            public string Name;
            public int[] Frames;
            public int FrameSpeed;
            public int currentIndex;

            public Animation(string name, int[] frames, int frameSpeed)
            {
                Name = name;
                Frames = frames;
                FrameSpeed = frameSpeed;
                currentIndex = 0;
            }
        }
    }
}

