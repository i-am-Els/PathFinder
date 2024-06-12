using System.Collections.Generic;
using System.Linq;
using Script.Artifacts;
using Script.Entities;
using Script.Pathfinding;
using Script.Selection;
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
        
        public void VisualizePath(Cell start, Cell dest)
        {
            SelectionService.SetEntitySelectionProperty(Color.white, start);
            // Update function to play move animation and check for obstacle set as it goes; 
            foreach (var path in _pathfindingService.Path)
            { 
                if (!GameSession.GetInstance().CanOperateAtCellVectorPosition(path.GetPosition())) continue;
                SelectionService.SetEntitySelectionProperty(Color.yellow, path);
            }
            SelectionService.SetEntitySelectionProperty(Color.blue, dest);
        }

        public List<Cell> GetPath() => _pathfindingService.Path;
        
        public void ResetPath() => _pathfindingService.Path = new List<Cell>();
    }
}