using System;
using Buildings;
using Utilities;
using UnityEngine;

namespace Managers
{
    public class PointAndClickManager : MonoBehaviourSinglenton<PointAndClickManager>
    {
        #region Inspector Variables

        [SerializeField] private LayerMask layerHitable;
        
        #endregion
        
        #region Private Variables

        private Camera _mainCamera; //Cached camera
        private Ray _ray;
        private RaycastHit _hit;
        private static LineRendererHighlight _currentHighlightedLineRenderer;
        private static LineRendererHighlight _selectedLineRenderer;

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
            _selectedLineRenderer?.Deselect();
            _currentHighlightedLineRenderer?.Unhighlight();
        }

        #endregion

        #region Private Methods

        private void HandlePointAndClickInput()
        {
            // Skip if right mouse button is pressed or UI is being interacted with
            if (Input.GetMouseButton(1) || Helpers.IsOverUI())
            {
                _currentHighlightedLineRenderer?.Unhighlight();
                return;
            }

            _ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(_ray.origin, _ray.direction * 100, Color.red);

            if (!Physics.Raycast(_ray, out _hit, 300, layerHitable)) return;

            HighlightHoveredTile();

            if (Input.GetMouseButtonDown(0)) // Left click
            {
                HandleTileSelection();
            }
        }

        private void HighlightHoveredTile()
        {
            if (!_hit.collider.TryGetComponent<LineRendererHighlight>(out var newLineRenderer)) return;

            switch (newLineRenderer.HighlightState)
            {
                case LineRendererHighlight.HighlightType.selected:
                    _currentHighlightedLineRenderer?.Unhighlight();
                    _currentHighlightedLineRenderer = newLineRenderer;
                    return;
                case LineRendererHighlight.HighlightType.highlighted:
                    _currentHighlightedLineRenderer.UpdateColor();
                    return;
                case LineRendererHighlight.HighlightType.none:
                    _currentHighlightedLineRenderer?.Unhighlight();
                    newLineRenderer.Highlight();
                    _currentHighlightedLineRenderer = newLineRenderer;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleTileSelection()
        {
            if (!_hit.collider.TryGetComponent<LineRendererHighlight>(out var newLineRenderer)) return;
            var buildType = _hit.collider.GetComponent<BuildType>().type;

            switch (BuildManager.Status)
            {
                case BuildManager.BuildingStatus.building:
                    if (buildType != BuildManager.BuildingType.none) return;
                    BuildManager.Instance.BuildBuilding(_hit.collider.gameObject);
                    break;
                case BuildManager.BuildingStatus.demolishing:
                    if (buildType == BuildManager.BuildingType.none) return;
                    MapManager.Instance.DemolishBuilding(buildType, _hit.collider.gameObject);
                    break;
                case BuildManager.BuildingStatus.none:
                    if (buildType == BuildManager.BuildingType.none) return;
                    if (HasSelectedTileChanged(newLineRenderer)) ShowSelectedTileInfo(buildType);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static bool HasSelectedTileChanged(LineRendererHighlight newLineRenderer)
        {
            if (newLineRenderer.HighlightState == LineRendererHighlight.HighlightType.selected) return false;
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.clickOnMap);
            _selectedLineRenderer?.Deselect();
            newLineRenderer.Select();
            _selectedLineRenderer = newLineRenderer;
            return true;
        }

        private void ShowSelectedTileInfo(BuildManager.BuildingType buildType)
        {
            UIManagerInGame.Instance.DisableAllPanels();
            UIManagerInGame.Instance.ShowInfoPanel(buildType, _hit.collider.gameObject);
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
            _selectedLineRenderer?.Deselect();

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