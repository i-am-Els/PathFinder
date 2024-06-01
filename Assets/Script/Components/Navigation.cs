using System.Collections.Generic;
using System.Linq;
using Script.Entities.Bases;
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
        
        public void SetNavigation(CellBase start, CellBase destination)
        {
            _pathfindingService.FindPath(start, destination);
        }
        
        public void VisualizePath()
        {
            // Update function to play move animation and check for obstacle set as it goes; 
            foreach (var path in from path in _pathfindingService.Path let v = path.GetVector() where GameSession.GetInstance().CanOperateAtCellVectorPosition(v) select path)
            {
                path.SetCellColorProperty(Color.yellow);
            }
        }

        public List<CellBase> GetPath() => _pathfindingService.Path;
        
        public void ResetPath() => _pathfindingService.Path = new List<CellBase>();
    }
}