using UnityEngine;

namespace Script
{
    [RequireComponent(typeof(GameSession))]
    [RequireComponent(typeof(InputManager))]
    public class GameManager : MonoBehaviour
    {

        private static GameManager _gameManager;

        private void Start()
        {
            _gameManager = this;
            
        }

        public static GameManager Instance()
        {
            return _gameManager;
        }
    }
}