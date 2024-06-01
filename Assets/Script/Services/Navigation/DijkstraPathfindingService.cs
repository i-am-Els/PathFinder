using System.Collections.Generic;
using Script.Entities.Bases;

namespace Script.Services.Navigation
{
    public class DijkstraPathfindingService : IPathfindingService
    {
        public void FindPath(CellBase start, CellBase destination)
        {
            
        }

        public List<CellBase> Path { get; set; }
    }
}