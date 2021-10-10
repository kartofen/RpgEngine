using System.Collections.Generic;
using RpgEngine.Entities;
namespace RpgEngine.Managers
{
    public static class EntityManager
    {
        public static List<IEntity> Entities = new List<IEntity>();
        public static void Update()
        {
            foreach(var entity in Entities)
                entity.Update();
        }

        public static void Unload()
        {
            foreach (var entity in Entities)
                entity.Unload();
        }

        public static void Draw()
        {
            foreach (var entity in Entities)
                entity.Draw();
        }

        public static void Clear()
        {
            Entities.Clear();
        }
    }
}