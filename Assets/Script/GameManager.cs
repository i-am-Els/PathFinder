using Script.Selection;
using UnityEngine;

namespace Script
{
    [RequireComponent(typeof(GameSession))]
    [RequireComponent(typeof(InputManager))]
    public class GameManager : MonoBehaviour
    {

        private static GameManager _gameManager;
        private readonly SelectionService _selectionService = new();

        private void Start()
        {
            GetComponent<InputManager>().SetSelectionService(_selectionService);
            _selectionService.SetCamera(Camera.main);
            _gameManager = this;
            
        }

        public static GameManager Instance()
        {
            return _gameManager;
        }
    }
}