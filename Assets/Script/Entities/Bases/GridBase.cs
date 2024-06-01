using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.Entities.Bases
{

    [ExecuteAlways]
    public class GridBase : MonoBehaviour
    {
        [SerializeField] private List<CellBase> cells = new();
        [SerializeField] private List<MobileAgent> mobileAgents = new();
        
        [SerializeField] private GameObject baseCellPrefab;

        
        private List<Vector2Int> _obstaclesPositions;

        [HideInInspector] public int widthInCellUnits;
        [HideInInspector] public int heightInCellUnits;
        
        public void GenerateGridCells()
        {
            foreach (var cell in cells)
            {
                DestroyImmediate(cell.gameObject);
            }
            cells.Clear();
            
            var gridWorldTransformPosition = transform.position;
            for (var i = 0; i < widthInCellUnits * heightInCellUnits; i++)
            {
                var cell = Instantiate(
                    baseCellPrefab,
                    new Vector3(gridWorldTransformPosition.x + Mathf.FloorToInt(i / (float)widthInCellUnits), 
                        gridWorldTransformPosition.y, 
                        gridWorldTransformPosition.z + Mathf.FloorToInt(i / (float)(heightInCellUnits))),
                    baseCellPrefab.transform.rotation);
                cell.transform.parent = gameObject.transform;
                var cellItem = cell.GetComponent<CellBase>();
                cellItem.InitCellWithData( Mathf.FloorToInt(i / (float)widthInCellUnits), Mathf.FloorToInt(i / (float)(heightInCellUnits)));
                cells.Add(cellItem); 
            }
        }
        
        public List<CellBase> GetCellItems() => cells;
        
        public List<MobileAgent> GetMobileAgents() => mobileAgents;
    }
}
