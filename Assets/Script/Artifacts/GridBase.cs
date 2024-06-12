using System;
using System.Collections.Generic;
using Script.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.Artifacts
{
    [ExecuteAlways]
    public class GridBase : MonoBehaviour
    {
        [SerializeField] private List<Cell> cells = new();
        
        // private List<Vector2Int> _obstaclesPositions;

        [HideInInspector] 
        public int widthInCellUnits;
        [HideInInspector] 
        public int heightInCellUnits;

        // public static Action<string> GridDeletedAction;
        
        // public static int Counter { get; private set; }

        public GridBase()
        {
            // Counter++;
        }
        
        /// <summary>
        /// Instantiates a grid's cell GameObjects and retrieves their Cell components into the _cells list. Returns void
        /// </summary>
        /// <param name="baseCellPrefab">The base GameObject to instantiate the cells with</param>
        public void GenerateGridCells(GameObject baseCellPrefab)
        {
            Debug.Log($"Cell instantiated: {widthInCellUnits}, {heightInCellUnits}");
            var gridWorldTransformPosition = transform.position;
            for (var i = 0; i < widthInCellUnits * heightInCellUnits; i++)
            {
                var cell = Instantiate(
                    baseCellPrefab,
                    new Vector3(gridWorldTransformPosition.x + (int)(i % (float)widthInCellUnits), 
                        gridWorldTransformPosition.y, 
                        gridWorldTransformPosition.z + (int)(i / (float)(widthInCellUnits))),
                    baseCellPrefab.transform.rotation);
                
                var temp = cell.GetComponent<Cell>();
                var cellItem = temp != null ? temp : cell.AddComponent<Cell>();
                cellItem.InitCellWithData( Mathf.FloorToInt(i % (float)widthInCellUnits), Mathf.FloorToInt(i / (float)(widthInCellUnits)));
                cell.name = $"Cell({cellItem.Row}, {cellItem.Col})";
                cells.Add(cellItem); 
                cell.transform.SetParent(transform);
                cellItem.SilentlySetCellType(ECellType.Pathway);
            }
        }
        
        public List<Cell> GetCellItems() => cells;

        // private void OnDestroy()
        // {
        //     GridDeletedAction.Invoke(name);
        // }
    }
}
