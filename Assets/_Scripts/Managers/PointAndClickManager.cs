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
        private LineRenderer _currentLineRenderer;
        private LineRenderer _currentLineRendererSelected;

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

        public void DisableCurrentLineRendererSelected()
        {
            if (_currentLineRendererSelected.IsNotNull())
            {
                DisableCurrentSelected();
            }

            if (_currentLineRenderer.IsNotNull())
            {
                _currentLineRenderer.enabled = false;
                _currentLineRenderer = null;
            }
        }

        #endregion

        #region Private Methods

        private void HandlePointAndClickInput()
        {
            // Skip if right mouse button is pressed or UI is being interacted with
            if(Input.GetMouseButton(1) || Helpers.IsOverUI())
            {
                if (_currentLineRenderer.IsNull()) return;
                _currentLineRenderer.enabled = false;
                _currentLineRenderer = null;
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
            if (_hit.collider.IsNull()) return;
            var newLineRenderer = _hit.collider.GetComponent<LineRenderer>();

            // If we're already highlighting something different
            if (_currentLineRenderer.IsNotNull() && _currentLineRenderer.gameObject.name.NotEquals(newLineRenderer.name))
            {
                // Don't disable current highlight if it's the selected one
                var isCurrentSelectedTile = _currentLineRendererSelected.IsNotNull() && newLineRenderer.name.Equals(_currentLineRendererSelected.gameObject.name);

                if (!isCurrentSelectedTile)
                {
                    var shouldDisableCurrent = _currentLineRendererSelected.IsNull() || _currentLineRenderer.gameObject.name.NotEquals(_currentLineRendererSelected.gameObject.name);

                    if (shouldDisableCurrent)
                    {
                        _currentLineRenderer.enabled = false;
                    }
                }

                _currentLineRenderer = newLineRenderer;
                newLineRenderer.enabled = true;
            }
            else if (_currentLineRenderer.IsNull())
            {
                _currentLineRenderer = newLineRenderer;
                _currentLineRenderer.enabled = true;
            }
        }

        private void HandleTileSelection()
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.clickOnMap);
            SelectTile();
            ProcessSelectedTile();
        }

        private void SelectTile()
        {
            var newLineRenderer = _hit.collider.GetComponent<LineRenderer>();

            // If we're selecting a different tile than the currently selected one
            if (_currentLineRendererSelected.IsNotNull() &&
                _currentLineRendererSelected.gameObject.name.NotEquals(newLineRenderer.gameObject.name))
            {
                _currentLineRendererSelected.enabled = false;
                _currentLineRendererSelected = newLineRenderer;
                newLineRenderer.enabled = true;
            }
            else if (_currentLineRendererSelected.IsNull())
            {
                _currentLineRendererSelected = newLineRenderer;
                _currentLineRendererSelected.enabled = true;
            }
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
            if (_currentLineRendererSelected.IsNotNull())
            {
                DisableCurrentSelected();
            }

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

        private void DisableCurrentSelected()
        {
            _currentLineRendererSelected.enabled = false;
            _currentLineRendererSelected = null;
        }

        #endregion
    }
}