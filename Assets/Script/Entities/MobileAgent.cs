using Script.Artifacts;
using Script.Components;
using Script.Enums;
using Script.Services.Navigation;
using Script.Services.Selection;
using UnityEngine;

namespace Script.Entities
{
    public class MobileAgent : SelectableEntity
    {
        private Navigation _navigation;
        [SerializeField] private Cell start;
        [SerializeField] private Cell destination;

        // public int Row { get; set; }
        // public int Col { get; set; }
        // public bool IsActivated { get; set; }
        
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
                // _navigation.VisualizePath();
            }else print("No Path Found");
        }
        
        private void PauseSim()
        {
            
        }

        private void CellChanged(Cell activeCell)
        {
            if (!_navigation.GetPath().Contains(activeCell) || activeCell.GetCellType() != ECellType.Pathway) return;
            _navigation.ResetPath();
            _navigation.SetNavigation(start, destination);
        }

        public Cell GetStartingPoint() =>  start;

        public Cell GetDestination() => destination;
        
        // public Vector2Int GetPosition() => new Vector2Int(Row, Col);
        
        public override void Select()
        {
            // throw new System.NotImplementedException("Getting to this in a moment");
        }

        public override void Deselect()
        {
            // throw new System.NotImplementedException("Getting to this in a moment");
        }
    }
}