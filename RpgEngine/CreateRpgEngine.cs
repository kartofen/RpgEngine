namespace RpgEngine
{
    public class CreateRpgEngine
    {
        public static int ScreenHeight { get; set; }
        public static int ScreenWidth  { get; set; }

        public CreateRpgEngine(int screenheight, int screenwidth)
        {
            ScreenHeight = screenheight;
            ScreenWidth = screenwidth;
        }

    }
}
