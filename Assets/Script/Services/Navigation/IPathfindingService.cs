using System.Collections.Generic;
using Script.Entities;
using Script.Entities.Bases;

namespace Script.Services.Navigation
{
    public interface IPathfindingService
    {
        /// <summary>
        /// Finds the Path from Start to Destination and updates the Path property in-place, sets IPathfindingService.Path to empty List of CellItem if no path is found.
        /// </summary>
        /// <param name="start">CellItem that depicts the starting point of a Mobile Agent in the Grid</param>
        /// <param name="destination">CellItem that depicts the destination cell of a Mobile Agent on the Grid</param>
        public void FindPath(CellBase start, CellBase destination);
        
        public List<CellBase> Path { get; set; }
    }
}