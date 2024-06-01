using Script.Components;
using Script.Entities.Bases;
using Script.Enums;
using Script.Services.Navigation;
using UnityEngine;

namespace Script.Entities
{
    public class MobileAgent : MonoBehaviour, IEntity
    {
        private Navigation _navigation;
        [SerializeField] private CellBase start;
        [SerializeField] private CellBase destination;

        public int Row { get; set; }
        public int Col { get; set; }
        public bool IsSelected { get; set; }
        
        private void OnEnable()
        {
            GameSession.GlobalResetSimulationTrigger += ResetSim;
            GameSession.PlaySimulationTrigger += PlaySim;
            GameSession.PauseSimulationTrigger += PauseSim;
            GameSession.CellChangedTrigger += CellChanged;
        }

        private void OnDisable()
        {
            GameSession.GlobalResetSimulationTrigger -= ResetSim;
            GameSession.PlaySimulationTrigger -= PlaySim;
            GameSession.PauseSimulationTrigger -= PauseSim;
            GameSession.CellChangedTrigger -= CellChanged;
        }

        private void Start()
        {
            _navigation = new Navigation(new AStarPathfindingService());
        }
        
        private void ResetSim()
        {
            _navigation.SetService(new AStarPathfindingService());
        }

        private void PlaySim()
        {
            _navigation.SetNavigation(start, destination);
            if (_navigation.GetPath().Count > 0)
            {
                _navigation.VisualizePath();
            }else print("No Path Found");
        }
        
        private void PauseSim()
        {
            
        }

        private void CellChanged(CellBase activeCell)
        {
            if (!_navigation.GetPath().Contains(activeCell) || activeCell.GetCellType() != ECellType.Pathway) return;
            _navigation.ResetPath();
            _navigation.SetNavigation(start, destination);
        }

        public CellBase GetStartingPoint() =>  start;

        public CellBase GetDestination() => destination;
        public Vector2Int GetVector() => new Vector2Int(Row, Col);
    }
}