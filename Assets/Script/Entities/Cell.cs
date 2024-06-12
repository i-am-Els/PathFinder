using System;
using Script.Artifacts;
using UnityEngine;

namespace Script.Entities
{
    [RequireComponent(typeof(MeshRenderer))]
    public class Cell : SelectableEntity
    { 
        // public int Row { get; set; } 
        //
        // public int Col { get; set; } 
        
        private ECellType _cellType;
        
        public static Action<int, int, string> CellDeletedAction;
        
        [HideInInspector] public GameObject visualItem;
        //
        // public bool IsActivated { get; set; } // Formally known as IsSelected
        
        public bool IsFluctuating { get; set; }  

        // For path retracing this is important
        [HideInInspector] public Cell parent;
        public int GCost { get; set; }
        public int HCost { get; set; }
        private int FCost => GCost + HCost;
        
        public void InitCellWithData(int rowData, int colData)
        {
            Row = rowData;
            Col = colData;
        }
        
        // public Vector2Int GetPosition() => new Vector2Int(Row, Col);

        private void OnEnable()
        {
            GameSession.GlobalResetSimulationTrigger += ResetSim;
        }

        private void OnDisable()
        {
            GameSession.GlobalResetSimulationTrigger -= ResetSim;
        }

        private void ResetSim()
        {
            // SetCellColorProperty(Color.gray);
            IsActivated = false;
        }

        public int GetFCost() => FCost;
        public ECellType GetCellType() => _cellType;
        
        public void SetCellType(ECellType value)
        {
            _cellType = value;
#if !UNITY_EDITOR
            GameSession.NotifyCellTypeChanged(this);
#endif
        }

        public void SilentlySetCellType(ECellType value)
        {
            _cellType = value;
        }

        // private void OnDestroy()
        // {
        //     transform.parent.gameObject.GetComponent<GridBase>()?.GetCellItems().Remove(this);
        //     CellDeletedAction.Invoke(Row, Col, gameObject.transform.parent.name);
        // }

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