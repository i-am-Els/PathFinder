using System;
using System.Collections.Generic;
using System.Linq;
using Script.Artifacts;
using Script.Enums;
using UnityEngine;

namespace Script.Services.Navigation
{
    public class AStarPathfindingService : IPathfindingService
    {
        public List<Cell> Path { get; set; } = new();

        public void FindPath(Cell start, Cell destination)
        {
            List<Cell> openList = new() { start };
            List<Cell> closedList = new();
            
            InitCosts();

            start.GCost = 0;
            start.HCost = GetManhattanHeuristic(start.GetPosition(), destination.GetPosition());
            
            while (openList.Count > 0)
            {
                var current = GetCellWithLowestFScore(openList);
                
                if (current == destination) Path = GetPath(destination);
                
                openList.Remove(current);
                closedList.Add(current);
                
                var neighbours = GetNeighbours(current);
                foreach (var n in neighbours)
                {
                    var someGCost = current.GCost + GetEuclideanDistance(current.GetPosition(), n.GetPosition());
                    if(someGCost < n.GCost)
                    {
                        n.GCost = someGCost;
                        n.parent = current;
                    }
                    
                    if (!openList.Contains(n)) openList.Add(n);
                }
            }
            
            Path = new List<Cell>();
            return;
            
            void InitCosts()
            {
                foreach (var cell in GameSession.GetGrid().GetCellItems())
                {
                    var activeCell = cell;
                    if (activeCell == null) continue;
                    activeCell.HCost = GetManhattanHeuristic(cell.GetPosition(), destination.GetPosition());
                    activeCell.GCost = int.MaxValue;
                    activeCell.parent = null;
                }
            }
            
            List<Cell> GetNeighbours(Cell current)
            {
                List<Cell> neighbours = new();
                var grid = GameSession.GetGrid();
                if (grid == null) return neighbours;
                
                for (var x = -1; x <= 1; x++)
                {
                    for (var y = -1; y <= 1; y++)
                    {
                        if (x == 0 && y == 0) continue;
                        
                        var neighbourX = current.Row + x;
                        var neighbourY = current.Col + y;
                        
                        if (neighbourX < 0 || neighbourX >= grid.widthInCellUnits || neighbourY < 0 ||
                            neighbourY >= grid.heightInCellUnits) continue;

                        var cell = GameSession.GetGrid().GetCellItems()[neighbourX + (neighbourY * grid.widthInCellUnits)];
                        if (cell is null) continue;
                        if (cell.GetCellType() != ECellType.Pathway) continue;
                        if (closedList.Contains(cell)) continue;
                        
                        neighbours.Add(cell);
                    }
                }
                return neighbours;
            }
        }


        private static List<Cell> GetPath(Cell destination)
        {
            List<Cell> path = new() { destination };
            var current = destination;
            while (current.parent != null)
            {
                path.Add(current.parent);
                current = current.parent;
            }
        
            path.Reverse();
            return path;
        }

        private static Cell GetCellWithLowestFScore(List<Cell> openList)
        {
            return openList.OrderBy(cell => cell.GetFCost()).First();
        }
        
        private static int GetManhattanHeuristic(Vector2Int current, Vector2Int destination)
        {
            return ((Mathf.Abs(current.x - destination.x) + Mathf.Abs(current.y - destination.y)) * 10);
        }
        
        private static int GetEuclideanDistance(Vector2Int current, Vector2Int destination)
        {
            var dist =  (int)(Math.Sqrt(
                Mathf.Pow((current.x - destination.x), 2) + 
                Mathf.Pow((current.y - destination.y),2)
            ) * 10);
            var grid = GameSession.GetGrid();
            if (dist != 10)
            {
                return (grid.widthInCellUnits * grid.heightInCellUnits * 10); // This is to prevent taking diagonal cells;
                // often times diagonal cells are left open barricaded by obstacles but the pathfinder jumps into the cells
                // in unrealistic manners that dont fit the way cars would move.
            }
            return dist;
        }
    }
}