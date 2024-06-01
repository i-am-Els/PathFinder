using Script.Enums;
using UnityEngine;

namespace Script.Entities.Bases
{
    [RequireComponent(typeof(MeshRenderer))]
    public class CellBase : MonoBehaviour
    {
        public int Row { get; set; }
        public int Col { get; set; }

        [SerializeField] private ECellType cellType;

        public void InitCellWithData(int rowData, int colData)
        {
            Row = rowData;
            Col = colData;
        }
        
        public Vector2Int GetVector()
        {
            return new Vector2Int(Row, Col);
        }
        
        public bool IsSelected { get; set; }
        
        public bool IsFluctuating { get; set; }  

        // For path retracing this is important
        [HideInInspector] public CellBase parent;
        public int GCost { get; set; }
        public int HCost { get; set; }
        
        private int FCost { get; set; }

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
            SetCellColorProperty(Color.gray);
            IsSelected = false;
        }

        public int GetFCost()
        {
            FCost = GCost + HCost;
            return FCost;
        }
        
        public void SetCellColorProperty(Color color)
        {
            gameObject.GetComponent<MeshRenderer>().material.color = color;
        }

        public ECellType GetCellType()
        {
            return cellType;
        }

        public void SetCellType(ECellType value)
        {
            cellType = value;
            GameSession.CellChangedTrigger.Invoke(this);
        }
    }
}