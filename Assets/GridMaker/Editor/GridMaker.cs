using System;
using GridMaker.Editor.Services.IO;
using Script.Enums;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GridMaker.Editor
{
    public class GridMaker : EditorWindow
    {
        private static FileInteractionService _fileInteractionService;

        private static TextField _gridTitleTextField;
        
        [MenuItem("CustomTools/GridMaker/Open Editor %&A")]
        public static void ShowEditor()
        {
            GridMaker wnd = GetWindow<GridMaker>();
            wnd.titleContent = new GUIContent("GridMaker");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/GridMaker/Editor/GridMaker.uxml");
            VisualElement labelFromUxml = visualTree.CloneTree();
            var mainLayoutContainer = labelFromUxml.Query<VisualElement>("mainLayoutContainer");
            // root.Add(labelFromUxml);
            rootVisualElement.Add(mainLayoutContainer);

            // ====================================  Get UI Items ======================================
            // Grid Details
            var gridTitle = rootVisualElement.Q("gridTitle") as TextField;
            _gridTitleTextField = gridTitle;
            var basePrefabField = rootVisualElement.Q("basePrefabField") as ObjectField;
            var rowSlider = rootVisualElement.Q("rowSlider") as SliderInt;
            var colSlider = rootVisualElement.Q("colSlider") as SliderInt;
            
            // Action Buttons
            var createButton = rootVisualElement.Q("createButton") as Button;
            var deleteButton = rootVisualElement.Q("deleteButton") as Button;
            
            // Grid Collection
            var gridCollectionArray = rootVisualElement.Q("gridCollectionArray") as ListView;
            
            // Cell Collection
            var cellListView = rootVisualElement.Q("cellListView") as ListView;
            
            // Cell Details
            var cellRow = rootVisualElement.Q("cellRow") as IntegerField;
            var cellCol = rootVisualElement.Q("cellCol") as IntegerField;
            var cellTypeEnumField = rootVisualElement.Q("cellTypeEnumField") as EnumField;
            var cellPrefabField = rootVisualElement.Q("cellPrefabField") as ObjectField;
            
            // Binding functionality to UI Elements
            // Grid Details
            rowSlider.RegisterCallback<ChangeEvent<int>>((evt) => AdjustGridOnSliderChanged(evt.newValue, colSlider.value));
            colSlider.RegisterCallback<ChangeEvent<int>>((evt) => AdjustGridOnSliderChanged(rowSlider.value, evt.newValue));
            
            createButton.RegisterCallback<ClickEvent>(evt => OnCreateButtonClicked());
            deleteButton.RegisterCallback<ClickEvent>(evt => OnDeleteButtonClicked());
            
            cellTypeEnumField.RegisterCallback<ChangeEvent<Enum>>(evt => OnCellTypeChanged(evt.newValue));

            // =================================== Bind Values =====================================
            // var data = new GridData
            // {
            //     Row = 1,
            //     Column = 2,
            //     Name = "Some Grid",
            //     BasePrefab = "Name"
            // };
        }

        private void OnBasePrefabChanged()
        {
            
        }

        private void AdjustGridOnSliderChanged(int row, int col)
        {
            // To delete or add cells as Slider value changes.
            _gridTitleTextField.value = $"Row: {row}, Col: {col}";
        }

        private void OnCellSelected(int row, int col)
        {     
            // populate cell details with details from the selected cell.
        }

        private void OnCreateButtonClicked()
        {
            
        }

        private void OnLoadButtonClicked()
        {
            
        }

        private void OnDeleteButtonClicked()
        {
            
        }

        private void OnCellTypeChanged(Enum cellType)
        {
            Debug.Log("Cell type changing");
            switch ((ECellType)cellType)
            {
                case ECellType.Obstacle:
                    break;
                case ECellType.Pathway:
                    break;
                case ECellType.LockedCell:
                    break;
            }
        }

        private void OnPrefabGridItemDoubleTapped()
        {
            // first change the value of the cell prefab then
            OnCellPrefabChanged();
        }
        
        private void OnCellPrefabChanged()
        {
            
        }

        private void ConfirmDelete(bool shouldDelete)
        {
            
        }

        private void OnTitleChanged(string newTitle)
        {
            
        }
    }
}