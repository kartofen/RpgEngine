using System;
using System.Collections.Generic;
using Raylib_cs;

namespace RpgEngine
{
    public static class ObjectManager 
    {
        public static List<dynamic> ObjectsList = new List<dynamic>();

        public static void Update()
        {
            foreach (var obj in ObjectsList)
                obj.Update();
        }

        public static void Clear()
        {
            ObjectsList.Clear();
        }
    }
}