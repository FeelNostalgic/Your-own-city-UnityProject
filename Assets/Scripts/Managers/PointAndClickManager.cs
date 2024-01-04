using Proyecto.Building;
using Proyecto.Utilities;
using UnityEngine;

namespace Proyecto.Managers
{
    public class PointAndClickManager : Singlenton<PointAndClickManager>
    {
        #region Inspector Variables

        [SerializeField] private LayerMask layerHitable;

        #endregion

        #region Public Variables

        [HideInInspector] public bool IsEnableRaycast;

        public bool IsGamePaused
        {
            get => _isGamePaused;
            set => _isGamePaused = value;
        }

        public bool IsGameOver
        {
            get => _isGameOver;
            set => _isGameOver = value;
        }

        #endregion

        #region Private Variables

        private Ray _ray;
        private RaycastHit _hit;
        private LineRenderer _currentLineRenderer;
        private LineRenderer _currentLineRendererSelected;
        private bool _isGamePaused;
        private bool _isGameOver;

        #endregion

        #region Unity Methods

        private void Start()
        {
            IsEnableRaycast = true;
            _isGamePaused = false;
            _isGameOver = false;
        }

        private void Update()
        {
            if (!_isGamePaused)
            {
                InputPointAndHitRaycast();
            }

            if (!_isGameOver && MapManager.Instance.IsMapCreated) InputKeyboard();
        }

        #endregion

        #region Public Methods

        public void DisableCurrentLineRenderer()
        {
            _currentLineRenderer.enabled = false;
        }

        #endregion

        #region Private Methods

        private void InputPointAndHitRaycast()
        {
            if (!Input.GetMouseButton(1) && IsEnableRaycast)
            {
                _ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(_ray, out _hit, 300, layerHitable))
                {
                    ShowHit();

                    if (Input.GetMouseButtonDown(0)) //Boton izquierdo
                    {
                        AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.clickOnMap);
                        ShowHitSelected();

                        BuildManager.Instance.SetCurrentTile(_hit.collider.gameObject);
                        if (_hit.collider.GetComponent<BuildType>().Type != BuildManager._building.none)
                        {
                            UIManager.Instance.DisableAllPanels();
                            UIManager.Instance.HideAllAreas();
                            UIManager.Instance.ShowPanelInfo(_hit.collider.GetComponent<BuildType>().Type,
                                _hit.collider);
                        }
                        else
                        {
                            UIManager.Instance.DisableAllPanels();
                            UIManager.Instance.HideAllAreas();
                            UIManager.Instance.ShowBuildPanel(true);
                        }
                    }
                }

                Debug.DrawRay(_ray.origin, _ray.direction * 100, Color.red);
            }
        }

        private void InputKeyboard()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (UIManager.Instance.IsAPanelActive && !_isGamePaused)
                {
                    if (_currentLineRendererSelected != null)
                    {
                        _currentLineRendererSelected.enabled = false;
                        _currentLineRendererSelected = null;
                    }

                    UIManager.Instance.DisableAllPanels();
                }
                else
                {
                    if (_isGamePaused) UIManager.Instance.PauseGame(false);
                    else UIManager.Instance.PauseGame(true);
                }
            }
        }

        private void ShowHit()
        {
            var newLineRenderer = _hit.collider.GetComponent<LineRenderer>();
            if (_currentLineRenderer != null && _currentLineRenderer.gameObject.name != newLineRenderer.name)
            {
                if (_currentLineRendererSelected != null)
                {
                    if (newLineRenderer.name != _currentLineRendererSelected.gameObject.name)
                    {
                        if (_currentLineRenderer.gameObject.name != _currentLineRendererSelected.gameObject.name)
                            _currentLineRenderer.enabled = false;
                        _currentLineRenderer = newLineRenderer;
                        newLineRenderer.enabled = true;
                    }
                }
                else
                {
                    _currentLineRenderer.enabled = false;
                    _currentLineRenderer = newLineRenderer;
                    newLineRenderer.enabled = true;
                }
            }
            else
            {
                if (_currentLineRenderer == null)
                {
                    _currentLineRenderer = newLineRenderer;
                    _currentLineRenderer.enabled = true;
                }
            }
        }

        private void ShowHitSelected()
        {
            var newLineRenderer = _hit.collider.GetComponent<LineRenderer>();
            if (_currentLineRendererSelected != null &&
                _currentLineRendererSelected.gameObject.name != newLineRenderer.gameObject.name)
            {
                _currentLineRendererSelected.enabled = false;
                _currentLineRendererSelected = newLineRenderer;
                newLineRenderer.enabled = true;
            }
            else
            {
                if (_currentLineRendererSelected == null)
                {
                    _currentLineRendererSelected = newLineRenderer;
                    _currentLineRendererSelected.enabled = true;
                }
            }
        }

        #endregion
    }
}