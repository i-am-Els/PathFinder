using System;
using System.Collections.Generic;
using System.Linq;
using GridMaker.Editor.Services.IO;
using Script.Artifacts;
using Script.Enums;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GridMaker.Editor
{
    public class GridMaker : EditorWindow
    {
        private FileInteractionService _fileInteractionService;
        private List<GridBase> _allGrids = new();
        private GameObject _selectedGrid;
        private GameObject _selectedCell;

        // ================================== UI Fields =================================
        private TextField _gridTitle;

        private SliderInt _rowSlider;
        private SliderInt _colSlider;

        private Button _createButton;
        private Button _saveButton;
        private Button _deleteButton;

        private ObjectField _basePrefabField;

        private ListView _gridCollectionArray;
        private ListView _cellListView;

        private IntegerField _cellRow;
        private IntegerField _cellCol;

        private EnumField _cellTypeEnumField;
        private ObjectField _cellPrefabField;
        private VisualElement _sceneView;
        
        public static Action<int, int> GridDeletedAction;
        //

        [MenuItem("CustomTools/GridMaker/Open Editor %&A")]
        public static void ShowEditor()
        {
            GridMaker wnd = GetWindow<GridMaker>();
            wnd.titleContent = new GUIContent("GridMaker");
        }

        private void OnEnable()
        {
            GridBase.GridDeletedAction += GridDeleted;
            Cell.CellDeletedAction += CellDeleted;
        }
        
        private void OnDestroy()
        {
            GridBase.GridDeletedAction -= GridDeleted;
            Cell.CellDeletedAction -= CellDeleted;
        }

        /// <summary>
        /// Called when a cell is deleted
        /// </summary>
        /// <param name="row">cell row</param>
        /// <param name="col">cell col</param>
        /// <param name="ownerName">Cell name</param>
        private void CellDeleted(int row, int col, string ownerName)
        {
            Debug.Log($"Cell Pos({row}, {col}) in {ownerName} is deleted");
        }

        /// <summary>
        /// To be called when any grid is deleted
        /// </summary>
        /// <param name="gridName">The name of the grid</param>
        private void GridDeleted(string gridName)
        {
            Debug.Log($"Refreshed after deleting {gridName}");
            ClearAllFields();
            LoadAllGrids();
        }

        /// <summary>
        /// Creates the Editor Window GUI Visual Elements
        /// </summary>
        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/GridMaker/Editor/GridMaker.uxml");
            VisualElement labelFromUxml = visualTree.CloneTree();
            var mainLayoutContainer = labelFromUxml.Q<VisualElement>("mainLayoutContainer");
            rootVisualElement.Add(mainLayoutContainer);

            // ====================================  Get UI Items ======================================
            GetAllUIElements();

            // Binding functionality to UI Elements
            RegisterAllUICallbacks();
            
            // =================================== Reset Values =====================================
            ClearAllFields();
            
            LoadAllGrids();
            
            // RenderGrid();
        }
        
        public void OnDisable()
        {
            UnRegisterAllUICallbacks();
        }

        private void GetAllUIElements()
        {
            // Grid Details
            _gridTitle = rootVisualElement.Q("gridTitle") as TextField;

            _rowSlider = rootVisualElement.Q("rowSlider") as SliderInt;
            _colSlider = rootVisualElement.Q("colSlider") as SliderInt;

            // Action Buttons
            _createButton = rootVisualElement.Q("createButton") as Button;
            _saveButton = rootVisualElement.Q("saveButton") as Button;
            _deleteButton = rootVisualElement.Q("deleteButton") as Button;

            //
            _basePrefabField = rootVisualElement.Q("basePrefabField") as ObjectField;

            // Grid Collection
            _gridCollectionArray = rootVisualElement.Q("gridCollectionArray") as ListView;

            // Cell Collection
            _cellListView = rootVisualElement.Q("cellListView") as ListView;

            // Cell Details
            _cellRow = rootVisualElement.Q("cellRow") as IntegerField;
            _cellCol = rootVisualElement.Q("cellCol") as IntegerField;
            _cellTypeEnumField = rootVisualElement.Q("cellTypeEnumField") as EnumField;
            _cellPrefabField = rootVisualElement.Q("cellPrefabField") as ObjectField;

            // Scroll View
            _sceneView = rootVisualElement.Q("scrollViewGrid");
        }

        private void RegisterAllUICallbacks()
        {
            _createButton?.RegisterCallback<ClickEvent>(buttonClickEvent => OnCreateButtonClicked());
            _saveButton?.RegisterCallback<ClickEvent>(buttonClickEvent => OnSaveButtonClicked());
            _deleteButton?.RegisterCallback<ClickEvent>(buttonClickEvent => OnDeleteButtonClicked());

            // _cellListView.onSelectionChange += CellListViewOnSelectionChange;

            _cellTypeEnumField?.RegisterCallback<ChangeEvent<Enum>>(fieldChangedEvent => OnCellTypeChanged(fieldChangedEvent.newValue));
            _cellPrefabField.RegisterValueChangedCallback(fieldChangedEvent =>
            {
                OnCellPrefabChanged((GameObject)fieldChangedEvent.newValue);
            });
            
            Selection.selectionChanged += SelectionChanged;
        }

        private void UnRegisterAllUICallbacks()
        {
           _createButton?.UnregisterCallback<ClickEvent>(buttonClickEvent => OnCreateButtonClicked());
           _saveButton?.UnregisterCallback<ClickEvent>(buttonClickEvent => OnSaveButtonClicked());
           _deleteButton?.UnregisterCallback<ClickEvent>(buttonClickEvent => OnDeleteButtonClicked());

           // _cellListView.onSelectionChange -= CellListViewOnSelectionChange;

           _cellTypeEnumField?.UnregisterCallback<ChangeEvent<Enum>>(fieldChangedEvent => OnCellTypeChanged(fieldChangedEvent.newValue));
           _cellPrefabField.UnregisterValueChangedCallback(fieldChangedEvent =>
           {
               OnCellPrefabChanged((GameObject)fieldChangedEvent.newValue);
           });
    
           Selection.selectionChanged -= SelectionChanged;
        }
        
        private void SelectionChanged()
        {
            if (Selection.count == 0 || Selection.activeGameObject == null || Selection.activeGameObject.GetComponent<GridBase>() == null) return;
            var grid = Selection.activeGameObject.GetComponent<GridBase>();
            Debug.Log("Grid Selected");
            // OnLoadGrid();
        }

        /// <summary>
        /// This event triggers when the a cell is selected in the Cell List View
        /// </summary>
        /// <param name="cell">The cell item</param>
        private void OnCellSelected(GameObject cell)
        {
            // populate cell details with details from the selected cell.
            _selectedCell = cell;
            UpdateDetails();
        }

        /// <summary>
        /// This event is triggered when the save button is clicked. It saves a grid to memory or updates if it already is in memory
        /// </summary>
        private void OnSaveButtonClicked()
        {
            if (!CanSave()) return; // Prompt A MessageBox here
            if (!IsEditing())
            {
                _selectedGrid = new GameObject();
                _selectedGrid.AddComponent<GridBase>();
                _selectedGrid.name = "Grid_" + GridBase.Counter;
            }
            var grid = _selectedGrid.GetComponent<GridBase>();
            grid.heightInCellUnits = _rowSlider.value;
            grid.widthInCellUnits = _colSlider.value;
            OnTitleChanged("Grid_" + GridBase.Counter);
            if (grid != null) grid.GenerateGridCells(_basePrefabField.value as GameObject);

            LoadAllGrids();
            CreateCellLists();
        }

        /// <summary>
        /// This event is triggered when the New button is clicked, it clears the fields and set selected grid to references an empty grid.
        /// </summary>
        private void OnCreateButtonClicked()
        {
            ClearAllFields();
            LoadAllGrids();
        }

        /// <summary>
        /// load every grid prefab
        /// </summary>
        private void LoadAllGrids()
        {
            _gridCollectionArray.Q<ScrollView>()?.Clear();
            var allGridsDict = FindPrefabsWithComponent<GridBase>();
            foreach (var grid in allGridsDict.Values.ToList())
            {
                var btn = new Button
                {
                    text = grid.name
                };
                btn.clicked += () => OnLoadGrid(grid);
                Debug.Log($"Tis' {grid.name}");
                _gridCollectionArray.Q<ScrollView>()?.Add(btn);
            }
        }

        /// <summary>
        /// Used to load a Grid when its selected.
        /// </summary>
        private void OnLoadGrid(GameObject grid)
        {
            ClearAllFields();
            var gridBase = grid.GetComponent<GridBase>();
            _rowSlider.value = gridBase.heightInCellUnits;
            _colSlider.value = gridBase.widthInCellUnits;
            OnTitleChanged(grid.name);
            _selectedGrid = grid;
            CreateCellLists();
        }

        /// <summary>
        /// Used to Trigger the Delete action
        /// </summary>
        private void OnDeleteButtonClicked()
        {

        }

        /// <summary>
        /// To toggle the CellType Enum
        /// </summary>
        /// <param name="cellType">New CellType</param>
        private void OnCellTypeChanged(Enum cellType)
        {
            if (_selectedGrid == null) return; // Prompt A MessageBox here
            if (_selectedCell == null) return; // Prompt A MessageBox here
            var cell = _selectedCell.GetComponent<Cell>();
            switch ((ECellType)cellType)
            {
                case ECellType.LockedCell:
                    cell.SetCellType(ECellType.LockedCell);
                    break;
                case ECellType.Obstacle:
                    cell.SetCellType(ECellType.Obstacle);
                    break;
                case ECellType.Pathway:
                    cell.SetCellType(ECellType.Pathway);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cellType), cellType, null);
            }
        }

        /// <summary>
        /// Triggered when the cell preferred prefab is changed.
        /// </summary>
        private void OnCellPrefabChanged(GameObject obj)
        {
            if (_selectedGrid == null || _selectedCell == null) return;
            var cell = _selectedCell.GetComponent<Cell>();
            cell.visualItem = obj;
        }

        /// <summary>
        /// A confirm deletion dialog
        /// </summary>
        /// <param name="shouldDelete">Dialog response</param>
        private void ConfirmDelete(bool shouldDelete)
        {

        }

        /// <summary>
        /// This resets the value of the grid Title when a new grid is selected;
        /// </summary>
        /// <param name="newTitle">The new grids title</param>
        private void OnTitleChanged(string newTitle)
        {
            _gridTitle.value = newTitle;
        }

        /// <summary>
        /// Updates the Cell Detail Sections
        /// </summary>
        private void UpdateDetails()
        {
            var cell = _selectedCell.GetComponent<Cell>();
            _cellRow.value = cell.Row;
            _cellCol.value = cell.Col;

            _cellTypeEnumField.value = cell.GetCellType();
            _cellPrefabField.value = cell.visualItem;
        }

        // /// <summary>
        // /// Gets all the Prefabs that are VisualCells and displays them in the scroll view
        // /// </summary>
        // private void RenderGrid()
        // {
        //     // Replace commented code with a small scene view of the Local space of the Grid, like jow it is done when we open a prefab
        //     
        //     _sceneView.Clear();
        //     
        //     var visualCells = FindPrefabsWithComponent<VisualCell>(); 
        //     foreach (var (guid, prefab) in visualCells)
        //     {
        //         
        //         var assetPath = AssetDatabase.GUIDToAssetPath(guid);
        //     
        //         var editor = UnityEditor.Editor.CreateEditor(prefab);
        //         var tex = editor.RenderStaticPreview(assetPath, null, 20, 20);
        //         DestroyImmediate(editor);
        //     
        //         _allVisualCells.Add(prefab);
        //         var image = new Image
        //         {
        //             image = tex
        //         };
        //         _sceneView.Add(image);
        //     }
        // }

        /// <summary>
        /// Clears every field in the UI and resets their value to default. It also sets grid reference to Null
        /// </summary>
        private void ClearAllFields()
        {
            // clearing code here
            _gridTitle.value = "NewGrid_Unsaved... Save grid will you...";
            _rowSlider.value = 1;
            _colSlider.value = 1;

            _cellListView.Q<ScrollView>()?.Clear();

            _cellRow.value = -1;
            _cellCol.value = -1;
            _cellTypeEnumField.value = ECellType.LockedCell;
            _cellPrefabField.value = null;

            _selectedGrid = null;
            _selectedCell = null;
        }

        /// <summary>
        /// This checks if the program is editing a grid in memory or is working with a fresh new grid
        /// </summary>
        /// <returns>Bool- Is Editing</returns>
        private bool IsEditing()
        {
            return _selectedGrid != null;
        }

        /// <summary>
        /// Check if the grid can be saved i.e no field is null
        /// </summary>
        /// <returns>Bool - True or False</returns>
        private bool CanSave()
        {
            return !(_rowSlider.value == 0 || _colSlider.value == 0 || _basePrefabField.value == null);
        }

        /// <summary>
        /// Creates the lists of Cells and Adds them to ListView
        /// </summary>
        private void CreateCellLists()
        {
            Debug.Log($"Oh! {_selectedGrid.name}");
            var cells = _selectedGrid.GetComponent<GridBase>().GetCellItems();
            Debug.Log($"Tis' {cells.Count}");
            foreach (var t in cells)
            {
                Debug.Log($"Oh! {t.gameObject.name}");
                var cell = t;
                var button = new Button { text = cell.gameObject.name };
                button.clicked += () => OnCellSelected(cell.gameObject);
                _cellListView.Q<ScrollView>()?.Add(button);
            }
        }

        private static Dictionary<string, GameObject> FindPrefabsWithComponent<T>() where T : Component
        {
            var result = new Dictionary<string, GameObject>();

            // Search for all prefabs
            var guids = AssetDatabase.FindAssets("t:Prefab");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                // Check if the prefab contains the specified component
                if (prefab != null && prefab.GetComponent<T>() != null)
                {
                    result.Add(guid, prefab);
                }
            }

            return result;
        }
    }
}