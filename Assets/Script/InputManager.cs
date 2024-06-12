using System;
using System.Collections.Generic;
using System.Linq;
using Script.Artifacts;
using Script.Entities;
using Script.Selection;
using Script.Services.Selection;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Script
{
    public class InputManager : MonoBehaviour
    {
        private ClickActions _clickAction;
        private SelectionService _selectionService;
        
        // private bool _overUI;
        private InputManager()
        {
        }

        public void SetSelectionService(SelectionService selectionService)
        {
            _selectionService = selectionService;
        }

        private void OnEnable()
        {
            _clickAction = new ClickActions();
            _clickAction.Enable();
        }
        
        private void Start()
        {
            _clickAction.ClickControls.Select.performed += StartSelectEntity;
            _clickAction.ClickControls.Deselect.performed += StartDeselect;
            _clickAction.ClickControls.Act.performed += ActOnGrid;
        }

        private void OnDisable()
        {
            _clickAction.ClickControls.Select.performed -= StartSelectEntity;
            _clickAction.ClickControls.Deselect.performed -= StartDeselect;
            _clickAction.ClickControls.Act.performed -= ActOnGrid;
            _clickAction.Disable();
        }

        private void StartSelectEntity(InputAction.CallbackContext ctx)
        {
            _selectionService.StartSelectEntity();
        }
        
        private void StartDeselect(InputAction.CallbackContext ctx)
        {
            _selectionService.StartDeselect();
        }

        private void ActOnGrid(InputAction.CallbackContext ctx)
        {
            if (SelectionService.SelectedEntities.Count <= 0) return;
            switch (GameSession.GetActionType())
            {
                case EActionType.AddObstacle:
                    foreach (var c in SelectionService.SelectedEntities.Select((cell) => cell.GetComponent<Cell>()))
                    {
                        if (!GameSession.GetInstance().CanOperateAtCellVectorPosition(c.GetPosition())) continue;
                        print("Adding Obstacles in progress");
                        if (c.GetCellType() == ECellType.Obstacle) continue;
                        SelectionService.SetEntitySelectionProperty(Color.red, c);
                        c.IsActivated = false;
                        c.SetCellType(ECellType.Obstacle);
                    }

                    SelectionService.SelectedEntities.Clear();
                    break;
                case EActionType.RemoveObstacle:
                    var cellsToRemove = new List<Cell>();
                    foreach (var c in SelectionService.SelectedEntities.Select(cell => cell.GetComponent<Cell>()))
                    {
                        if (!GameSession.GetInstance().CanOperateAtCellVectorPosition(c.GetPosition())) continue;
                        if (c.GetCellType() != ECellType.Obstacle) continue;
                        print("Removing Obstacles in progress");
                        SelectionService.SetEntitySelectionProperty(Color.gray, c);
                        c.IsActivated = false;
                        c.SetCellType(ECellType.Pathway);
                        cellsToRemove.Add(c);
                    }
                    // _selectedCells.RemoveAll(cell => cellsToRemove.Contains(cell.GetComponent<CellItem>()));
                    foreach (var cell in cellsToRemove)
                    {
                        SelectionService.SelectedEntities.Remove(cell);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static List<SelectableEntity> GetSelectedCellItems()
        {
            return SelectionService.SelectedEntities;
        }
    }
}