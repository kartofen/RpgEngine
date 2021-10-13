using System.Collections.Generic;
using RpgEngine.Entities;
using RpgEngine.Renderer;

namespace RpgEngine.Managers
{
    public static class ObjectManager
    {
        public static List<IObject> Objects = new List<IObject>();

        public static void Update(ref Tilemap tilemap)
        {
            foreach (var obj in Objects)
                obj.Update(ref tilemap);
        }

        public static void Unload()
        {
            foreach (var obj in Objects)
                obj.Unload();
        }

        public static void Draw()
        {
            foreach (var obj in Objects)
                obj.Draw(); 
        }

        public static void Clear()
        {
            // objects with the ShouldRemove flag false have something to do (for example animations) so remove them later
            // because every frame i clear and then fill the ObjectsList 

            List<IObject> objectsToRemove = new List<IObject>();

            foreach (var obj in Objects)
                if(obj.ShouldRemove) objectsToRemove.Add(obj);

            foreach (var obj in objectsToRemove)
                Objects.Remove(obj);
            
            objectsToRemove.Clear();
        }
    }
}