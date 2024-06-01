using UnityEngine;

namespace Script.Entities
{
    public interface IEntity
    {
        public int Row { get; set; }
        public int Col { get; set; }
        
        public bool IsSelected { get; set; }

        public Vector2Int GetVector();
    }
}