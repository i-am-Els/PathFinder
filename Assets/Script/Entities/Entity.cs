using UnityEngine;

namespace Script.Entities
{
    public abstract class Entity : MonoBehaviour
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public bool IsActivated { get; set; }

        public Vector2Int GetPosition() => new (Row, Col);
        
    }
}