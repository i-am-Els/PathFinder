using System.Collections.Generic;
using System.Linq;
using Script.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Script.Selection
{
    public class SelectionService
    {
        public static List<SelectableEntity> SelectedEntities;
        
        private Camera _mainCamera;

        public SelectionService()
        {
            SelectedEntities = new List<SelectableEntity>();
        }

        public void SetCamera(Camera camera)
        {
            _mainCamera = camera;
        }

        public static void SetEntitySelectionProperty(Color color, SelectableEntity entity)
        {
            entity.GetComponent<MeshRenderer>().material.color = color;
        }
        
        public void StartSelectEntity()
        {
            DetectEntityOnClick(out var hitResult);
            if (!hitResult) return;
         
            if (hitResult.IsActivated) return;
            if (!GameSession.GetInstance().CanOperateAtCellVectorPosition(hitResult.GetPosition())) return;
            var grid = GameSession.GetGrid();
            if (SelectedEntities.Count >= (grid.widthInCellUnits * grid.heightInCellUnits) / 2)
            {
                // print("Too many selections"); 
                return;
            }
            hitResult.IsActivated = true;
            SelectedEntities.Add(hitResult);
            SetEntitySelectionProperty(Color.green, hitResult);
         
        }

        public void StartDeselect()
        {
            DetectEntityOnClick(out var result);
            if (!result)
            {
                DeselectAllEntities();
                return;
            } 
            DeselectEntities(result);
        }

        private void DetectEntityOnClick(out SelectableEntity result)
        {
            var ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out var hit))
            {
                result = null;
                return;
            }

            result = hit.collider.gameObject.GetComponent<SelectableEntity>();
        }

        private void DeselectAllEntities()
        {
            foreach (var cell in SelectedEntities.Where(cell => GameSession.GetInstance().CanOperateAtCellVectorPosition(cell.GetPosition())))
            {
                cell.IsActivated = false;
                SetEntitySelectionProperty((cell as Cell)?.GetCellType() != ECellType.Obstacle ? Color.gray : Color.red, cell);
            }

            SelectedEntities.Clear();
        }
        
        private void DeselectEntities(SelectableEntity activeCell)
        {
            if (!GameSession.GetInstance().CanOperateAtCellVectorPosition(activeCell.GetPosition())) return;
            if (!activeCell.IsActivated) return;
            SetEntitySelectionProperty((activeCell as Cell)?.GetCellType() != ECellType.Obstacle ? Color.gray : Color.red, activeCell);
            activeCell.IsActivated = false;
            SelectedEntities.Remove(activeCell as Cell);
        }
    }
}