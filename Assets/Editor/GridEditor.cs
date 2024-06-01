using Script.Entities;
using Script.Entities.Bases;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(GridBase))]
    [InitializeOnLoad]
    public class GridEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            GridBase grid = (GridBase)target;
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Grid Settings", EditorStyles.boldLabel);

            grid.heightInCellUnits = EditorGUILayout.IntSlider("Rows", grid.heightInCellUnits, 5, 100);
            grid.widthInCellUnits = EditorGUILayout.IntSlider("Columns", grid.widthInCellUnits, 5, 100);
            
            if (GUILayout.Button("Generate Grid"))
            {
                grid.GenerateGridCells();
            }
        }
    }
}