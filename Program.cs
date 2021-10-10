namespace RpgGameRaylib
{    
    static class Program
    {       
        public static int ScreenHeight = 720;
        public static int ScreenWidth = 1280;

        public static void Main()
        {
            new Game(ScreenHeight, ScreenWidth);
        }
    }
}
