using System;
using Buildings;
using Commons;
using Utilities;
using UnityEngine;

namespace Managers
{
    public class PointAndClickManager : MonoBehaviourSingleton<PointAndClickManager>
    {
        #region Inspector Variables

        [SerializeField] private LayerMask layerHitable;

        #endregion

        #region Private Variables

        private Camera _mainCamera; //Cached camera
        private Ray _ray;
        private RaycastHit _hit;
        private static TileFunctionality _currentHighlightedTile;
        private static TileFunctionality _selectedTile;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _mainCamera = Helpers.Camera;
        }

        private void Update()
        {
            if (GameManager.CurrentGameState == GameState.Playing)
            {
                HandlePointAndClickInput();
            }

            if (MapManager.Instance.IsMapCreated)
            {
                HandleKeyboardInput();
            }
        }

        #endregion

        #region Public Methods

        public static void DisableCurrentLineRendererSelected()
        {
            _selectedTile?.Deselect();
            _currentHighlightedTile?.Unhighlight();
        }

        #endregion

        #region Private Methods

        private void HandlePointAndClickInput()
        {
            // Skip if right mouse button is pressed or UI is being interacted with
            if (Input.GetMouseButton(1) || Helpers.IsOverUI())
            {
                _currentHighlightedTile?.Unhighlight();
                _currentHighlightedTile = null;
                BuildManager.DisablePreview();
                return;
            }

            _ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(_ray.origin, _ray.direction * 300, Color.red);

            if (!Physics.Raycast(_ray, out _hit, 300, layerHitable)) return;

            HighlightHoveredTile();

            if (Input.GetMouseButtonDown(0)) // Left click
            {
                HandleTileSelection();
            }
        }

        private void HighlightHoveredTile()
        {
            if (!MapManager.Instance.TryGetTile(_hit.collider.transform.position, out var newTile))
            {
                _currentHighlightedTile?.Unhighlight();
                _currentHighlightedTile = null;
                BuildManager.DisablePreview();
                return;
            }

            // Debug.Log($"Pointing to: {_hit.collider.name} | Get tile: {newTile.name} | World position: {_hit.collider.transform.position}, Map: {newTile.MapPosition}");

            switch (newTile.HighlightState)
            {
                case HighlightType.selected:
                    _currentHighlightedTile?.Unhighlight();
                    _currentHighlightedTile = newTile;
                    return;
                case HighlightType.highlighted:
                    _currentHighlightedTile.UpdateColor();
                    return;
                case HighlightType.none:
                    _currentHighlightedTile?.Unhighlight();
                    newTile.Highlight();
                    _currentHighlightedTile = newTile;
                    BuildManager.Instance.ShowBuildingPreview(newTile);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleTileSelection()
        {
            if (!MapManager.Instance.TryGetTile(_hit.collider.transform.position, out var selectedTile)) return;

            switch (BuildManager.Status)
            {
                case BuildingStatus.building:
                    if (selectedTile.BuildingType != BuildingType.none) return;
                    BuildManager.Instance.BuildBuilding(selectedTile);
                    break;
                case BuildingStatus.demolishing:
                    if (selectedTile.BuildingType == BuildingType.none) return;
                    MapManager.DemolishBuilding(selectedTile);
                    break;
                case BuildingStatus.none:
                    if (selectedTile.BuildingType == BuildingType.none) return;
                    if (HasSelectedTileChanged(selectedTile)) ShowSelectedTileInfo(selectedTile);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static bool HasSelectedTileChanged(TileFunctionality tile)
        {
            if (tile.HighlightState == HighlightType.selected) return false;
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.clickOnMap);
            _selectedTile?.Deselect();
            tile.Select();
            _selectedTile = tile;
            return true;
        }

        private void ShowSelectedTileInfo(TileFunctionality tile)
        {
            UIManagerInGame.Instance.DisableAllPanels();
            UIManagerInGame.Instance.ShowInfoPanel(tile);
        }

        private void HandleKeyboardInput()
        {
            if (!Input.GetKeyDown(KeyCode.Escape))
            {
                return;
            }

            if (UIManagerInGame.Instance.IsAPanelActive && GameManager.CurrentGameState == GameState.Playing)
            {
                CloseActivePanel();
            }
            else
            {
                TogglePauseState();
            }
        }

        private void CloseActivePanel()
        {
            _selectedTile?.Deselect();

            UIManagerInGame.Instance.DisableAllPanels();
        }

        private static void TogglePauseState()
        {
            switch (GameManager.CurrentGameState)
            {
                case GameState.Playing:
                    UIManagerInGame.Instance.PauseGame();
                    break;
                case GameState.Paused:
                    UIManagerInGame.Instance.UnpauseGame();
                    break;
            }
        }

        #endregion
    }
}