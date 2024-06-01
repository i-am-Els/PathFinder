using System;
using System.Collections.Generic;
using System.Linq;
using Script.Components;
using Script.Entities;
using Script.Entities.Bases;
using Script.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    [RequireComponent(typeof(UserInterface))]
    public class GameSession : MonoBehaviour
    {
        private static GameSession _instance;
        
        public GameObject grid;
        private static GridBase _gridBase;
        private static EActionType _actionType;
        private static bool _gameIsPaused = true;

        public List<GameObject> mobileAgents;

        private UserInterface _userInterface;

        public static Action GlobalResetSimulationTrigger;
        public static Action PlaySimulationTrigger;
        public static Action PauseSimulationTrigger;
        public static Action<CellBase> CellChangedTrigger;
        
        private void Start()
        {
            _gridBase = grid.GetComponent<GridBase>();
            _userInterface = GetComponent<UserInterface>();
            _instance = this;
            
            _userInterface.PlayAndPauseSimTrigger += PlayAndPauseSimulation;
            _userInterface.ResetSimTrigger += ResetSimulation;
            _userInterface.SetActionModeTrigger += SetActionMode;
        }
        
        private void OnDisable()
        {
            _userInterface.PlayAndPauseSimTrigger -= PlayAndPauseSimulation;
            _userInterface.ResetSimTrigger -= ResetSimulation;
            _userInterface.SetActionModeTrigger -= SetActionMode;
        }

        public static GameSession GetInstance()
        {
            return _instance;
        }
        
        public static GridBase GetGrid()
        {
            return _gridBase;
        }
        
        private static void SetActionMode(int option)
        {
            _actionType = option switch
            {
                0 => EActionType.AddObstacle,
                1 => EActionType.RemoveObstacle,
                _ => _actionType
            };
            print($"Changing Action Type to {_actionType}");
        }

        private static void ResetSimulation()
        {
            GlobalResetSimulationTrigger.Invoke();
            print("Resetting Simulation");
        }

        private void PlayAndPauseSimulation(Button  playAndPauseSim, Sprite sprite)
        {
            var spriteComponent = playAndPauseSim.GetComponent<Image>();
            if (!_gameIsPaused)
            {
                _gameIsPaused = true;
                spriteComponent.sprite = sprite;
                PauseSimulationTrigger.Invoke();
                print("Pausing Simulation");
            }
            else
            {
                _gameIsPaused = false;
                spriteComponent.sprite = sprite;
                PlaySimulationTrigger.Invoke();
                print("Playing Simulation");
            }
        }

        public static bool GameIsPaused()
        {
            return _gameIsPaused;
        }
        
        public static EActionType GetActionType()
        {
            return _actionType;
        }
        
        public bool CanOperateAtCellVectorPosition(Vector2Int cellPosition)
        {
            return mobileAgents.Select(agent => agent.GetComponent<MobileAgent>())
                .All(agentComponent => agentComponent.GetStartingPoint().GetVector() != cellPosition 
                                       && agentComponent.GetDestination().GetVector() != cellPosition);
        }
    }
}