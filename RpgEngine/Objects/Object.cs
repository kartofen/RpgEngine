using System.Collections.Generic;

namespace RpgEngine
{
    public abstract class Object 
    {
        public delegate void RemoveProperty(string propertyName, int tilesetToDrawIndex, int tileIndex);
        public static event RemoveProperty OnRemoveProperty;
        public void StartRemovePropertyEvent(string propertyName, int tilesetToDrawIndex, int tileIndex)
        {
            OnRemoveProperty(propertyName, tilesetToDrawIndex, tileIndex);
        }

        public delegate void RemoveTile(int indexOfLayer, int indexOfChunk, int indexInChunk);
        public static event RemoveTile OnRemoveTile;
        public void StartRemoveTileEvent(int indexOfLayer, int indexOfChunk, int indexInChunk)
        {
            OnRemoveTile(indexOfLayer, indexOfChunk, indexInChunk);
        }

        public virtual void Update() { }
    }
}