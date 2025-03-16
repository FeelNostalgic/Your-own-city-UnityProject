using System;
using Buildings;
using Utilities;
using UnityEngine;

namespace Managers
{
    public class PointAndClickManager : MonoBehaviourSinglenton<PointAndClickManager>
    {
        [SerializeField] private LayerMask layerHitable;

        private Camera _mainCamera;
        private Ray _ray;
        private RaycastHit _hit;
        private LineRendererHighlight _currentHighlightedLineRenderer;
        private LineRendererHighlight _selectedLineRenderer;

        #region Unity Methods

        private void Awake()
        {
            _mainCamera = Helpers.Camera;
        }

        private void Start()
        {
            MapManager.OnTileDelete += OnTileDeleted;
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

        public void DisableCurrentLineRendererSelected()
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
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.clickOnMap);
            SelectTile();
        }

        private void SelectTile()
        {
            if (!_hit.collider.TryGetComponent<LineRendererHighlight>(out var newLineRenderer)) return;

            if (newLineRenderer.HighlightState == LineRendererHighlight.HighlightType.selected) return;
            _selectedLineRenderer?.Deselect();
            newLineRenderer.Select();
            _selectedLineRenderer = newLineRenderer;
            //New selection then show new data
            ProcessSelectedTile(); 
        }

        private void ProcessSelectedTile()
        {
            BuildManager.Instance.SetCurrentTile(_hit.collider.gameObject);
            UIManagerInGame.Instance.DisableAllPanels();
            UIManagerInGame.Instance.HideAllAreas();

            var buildType = _hit.collider.GetComponent<BuildType>().type;
            if (buildType != BuildManager.BuildingType.none)
            {
                UIManagerInGame.Instance.ShowInfoPanel(buildType, _hit.collider);
            }
            else
            {
                UIManagerInGame.Instance.ShowBuildPanel(true);
            }
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

        private void TogglePauseState()
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
        
        private void OnTileDeleted(GameObject obj)
        {
            _selectedLineRenderer = null;
            _currentHighlightedLineRenderer = null;
        }
        
        #endregion
    }
}