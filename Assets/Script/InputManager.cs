using System;
using System.Collections.Generic;
using System.Linq;
using Script.Entities;
using Script.Entities.Bases;
using Script.Enums;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Script
{
    public class InputManager : MonoBehaviour
    {
        private ClickActions _clickAction;
        private Camera _mainCamera;
        private static List<CellBase> _selectedCells;
        
        private bool _overUI;
        private void OnEnable()
        {
            _clickAction = new ClickActions();
            _clickAction.Enable();
            _selectedCells = new List<CellBase>();
        }
        
        private void Start()
        {
            _mainCamera = Camera.main;
            _clickAction.ClickControls.Select.performed += StartSelectEntity;
            _clickAction.ClickControls.Deselect.performed += StartDeselect;
            _clickAction.ClickControls.Act.performed += ActOnGrid;
        }

        private void Update()
        {
            // _overUI = EventSystem.current.IsPointerOverGameObject();
            _overUI = false;
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
            DetectEntityOnClick(out var entity);
            if (!entity.Hit) return;
            if (entity.Cell)
            {
                if (entity.Cell.IsSelected) return;
                if (!GameSession.GetInstance().CanOperateAtCellVectorPosition(entity.Cell.GetVector())) return;
                var grid = GameSession.GetGrid();
                if (_selectedCells.Count >= (grid.widthInCellUnits * grid.heightInCellUnits)/2) { print("Too many selections"); return;}
                entity.Cell.IsSelected = true;
                _selectedCells.Add(entity.Cell);
                entity.Cell.SetCellColorProperty(Color.green);
                print("Cell Hit Detected!");  
            }else if(entity.Agent)
            {
                if(entity.Agent.IsSelected) return;
                print("Mobile Agent Selected");
            }
        }

        private void StartDeselect(InputAction.CallbackContext ctx)
        {
            DetectEntityOnClick(out var result);
            if (!result.Hit)
            {
                DeselectAllCells();
                return;
            }
            if (result.Cell)
                DeselectCell(result.Cell);
        }

        private void DetectEntityOnClick(out SHitResult result)
        {
            var ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out var hit) || _overUI)
            {
                result = new SHitResult{ Agent = null, Cell = null, Hit = false};
                return;
            }

            result = hit.collider.gameObject.GetComponent<CellBase>() ? 
                new SHitResult { Cell = hit.collider.gameObject.GetComponent<CellBase>(), Agent = null, Hit = true } : 
                new SHitResult { Cell = null, Agent = hit.collider.gameObject.GetComponent<MobileAgent>(), Hit = true };
        }

        private void DeselectAllCells()
        {
            foreach (var cell in _selectedCells.Where(cell => GameSession.GetInstance().CanOperateAtCellVectorPosition(cell.GetVector())))
            {
                cell.IsSelected = false;
                cell.SetCellColorProperty(cell.GetCellType() != ECellType.Obstacle ? Color.gray : Color.red);
            }

            _selectedCells.Clear();
        }
        
        private void DeselectCell(CellBase activeCell)
        {
            if (!GameSession.GetInstance().CanOperateAtCellVectorPosition(activeCell.GetVector())) return;
            if (!activeCell.IsSelected) return;
            activeCell.SetCellColorProperty(activeCell.GetCellType() != ECellType.Obstacle ? Color.gray : Color.red);
            activeCell.IsSelected = false;
            _selectedCells.Remove(activeCell);
        }

        private void ActOnGrid(InputAction.CallbackContext ctx)
        {
            if (_selectedCells.Count <= 0) return;
            switch (GameSession.GetActionType())
            {
                case EActionType.AddObstacle:
                    foreach (var c in _selectedCells.Select(cell => cell.GetComponent<CellBase>()))
                    {
                        if (!GameSession.GetInstance().CanOperateAtCellVectorPosition(c.GetVector())) continue;
                        print("Adding Obstacles in progress");
                        if (c.GetCellType() == ECellType.Obstacle) continue;
                        c.SetCellColorProperty(Color.red);
                        c.IsSelected = false;
                        c.SetCellType(ECellType.Obstacle);
                    }
                    _selectedCells.Clear();
                    break;
                case EActionType.RemoveObstacle:
                    var cellsToRemove = new List<CellBase>();
                    foreach (var c in _selectedCells.Select(cell => cell.GetComponent<CellBase>()))
                    {
                        if (!GameSession.GetInstance().CanOperateAtCellVectorPosition(c.GetVector())) continue;
                        if (c.GetCellType() != ECellType.Obstacle) continue;
                        print("Removing Obstacles in progress");
                        c.SetCellColorProperty(Color.gray);
                        c.IsSelected = false;
                        c.SetCellType(ECellType.Pathway);
                        cellsToRemove.Add(c);
                    }
                    // _selectedCells.RemoveAll(cell => cellsToRemove.Contains(cell.GetComponent<CellItem>()));
                    foreach (var cell in cellsToRemove)
                    {
                        _selectedCells.Remove(cell);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static List<CellBase> GetSelectedCellItems()
        {
            return _selectedCells;
        }
    }
}