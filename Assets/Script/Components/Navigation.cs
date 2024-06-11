using System.Collections.Generic;
using System.Linq;
using Script.Artifacts;
using Script.Services.Navigation;
using UnityEngine;

namespace Script.Components
{
    public class Navigation 
    {
        private IPathfindingService _pathfindingService;
        public Navigation(IPathfindingService pathfindingService)
        {
            _pathfindingService = pathfindingService;
        }

        public void SetService(IPathfindingService pathfindingService)
        {
            _pathfindingService = pathfindingService;
        }
        
        public void SetNavigation(Cell start, Cell destination)
        {
            _pathfindingService.FindPath(start, destination);
        }
        
        // public void VisualizePath()
        // {
        //     // Update function to play move animation and check for obstacle set as it goes; 
        //     foreach (var path in from path in _pathfindingService.Path let v = path.GetPosition() where GameSession.GetInstance().CanOperateAtCellVectorPosition(v) select path)
        //     {
        //         path.SetCellColorProperty(Color.yellow);
        //     }
        // }

        public List<Cell> GetPath() => _pathfindingService.Path;
        
        public void ResetPath() => _pathfindingService.Path = new List<Cell>();
    }
}