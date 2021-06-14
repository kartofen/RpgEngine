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
}
