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

        #region Public Variables

        public bool IsEnableRaycast { get; set; }
        
        #endregion

        #region Private Variables

        private Ray _ray;
        private RaycastHit _hit;
        private LineRenderer _currentLineRenderer;
        private LineRenderer _currentLineRendererSelected;
        private bool _isGamePaused;
        private bool _isGameOver;
        private Camera _mainCamera;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

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
            if (Input.GetMouseButton(1) || !IsEnableRaycast) return;
            _ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(_ray, out _hit, 300, layerHitable))
            {
                ShowHit();

                if (Input.GetMouseButtonDown(0)) //Left click
                {
                    AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.clickOnMap);
                    ShowHitSelected();

                    BuildManager.Instance.SetCurrentTile(_hit.collider.gameObject);
                    if (_hit.collider.GetComponent<BuildType>().Type != BuildManager._building.none)
                    {
                        UIManagerInGame.Instance.DisableAllPanels();
                        UIManagerInGame.Instance.HideAllAreas();
                        UIManagerInGame.Instance.ShowPanelInfo(_hit.collider.GetComponent<BuildType>().Type,
                            _hit.collider);
                    }
                    else
                    {
                        UIManagerInGame.Instance.DisableAllPanels();
                        UIManagerInGame.Instance.HideAllAreas();
                        UIManagerInGame.Instance.ShowBuildPanel(true);
                    }
                }
            }

            Debug.DrawRay(_ray.origin, _ray.direction * 100, Color.red);
        }

        private void InputKeyboard()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (UIManagerInGame.Instance.IsAPanelActive && !_isGamePaused)
                {
                    if (_currentLineRendererSelected != null)
                    {
                        _currentLineRendererSelected.enabled = false;
                        _currentLineRendererSelected = null;
                    }

                    UIManagerInGame.Instance.DisableAllPanels();
                }
                else
                {
                    if (_isGamePaused) UIManagerInGame.Instance.PauseGame(false);
                    else UIManagerInGame.Instance.PauseGame(true);
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