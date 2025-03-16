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

        //public bool IsEnableRaycast { get; set; }

        #endregion

        #region Private Variables

        private Ray _ray;
        private RaycastHit _hit;
        private LineRenderer _currentLineRenderer;
        private LineRenderer _currentLineRendererSelected;
        private Camera _mainCamera;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _mainCamera = Helpers.Camera;
        }

        private void Start()
        {
            //IsEnableRaycast = true;
        }

        private void Update()
        {
            if (GameManager.CurrentGameState == GameState.Playing)
            {
                InputPointAndHitRaycast();
            }

            if (MapManager.Instance.IsMapCreated) InputKeyboard();
        }

        #endregion

        #region Public Methods

        public void DisableCurrentLineRendererSelected()
        {
            if (_currentLineRendererSelected.IsNotNull())
            {
                DisableCurrentSelected();
            }
            
            if(_currentLineRenderer.IsNotNull()) _currentLineRenderer.enabled = false;
        }

        #endregion

        #region Private Methods

        private void InputPointAndHitRaycast()
        {
            if (Input.GetMouseButton(1) || Helpers.IsOverUI()) return;
            _ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(_ray, out _hit, 300, layerHitable))
            {
                ShowHit();

                if (Input.GetMouseButtonDown(0)) //Left click
                {
                    AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.clickOnMap);
                    ShowHitSelected();

                    BuildManager.Instance.SetCurrentTile(_hit.collider.gameObject);
                    if (_hit.collider.GetComponent<BuildType>().Type != BuildManager.BuildingType.none)
                    {
                        UIManagerInGame.Instance.DisableAllPanels();
                        UIManagerInGame.Instance.HideAllAreas();
                        UIManagerInGame.Instance.ShowPanelInfo(_hit.collider.GetComponent<BuildType>().Type, _hit.collider);
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
            if (!Input.GetKeyDown(KeyCode.Escape)) return;

            if (UIManagerInGame.Instance.IsAPanelActive && GameManager.CurrentGameState == GameState.Playing)
            {
                if (_currentLineRendererSelected.IsNotNull())
                {
                    DisableCurrentSelected();
                }

                UIManagerInGame.Instance.DisableAllPanels();
            }
            else
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
        }

        private void ShowHit()
        {
            var newLineRenderer = _hit.collider.GetComponent<LineRenderer>();
            if (_currentLineRenderer.IsNotNull() && _currentLineRenderer.gameObject.name.NotEquals(newLineRenderer.name))
            {
                if (_currentLineRendererSelected.IsNotNull())
                {
                    if (newLineRenderer.name.Equals(_currentLineRendererSelected.gameObject.name)) return;
                    if (_currentLineRenderer.gameObject.name.NotEquals(_currentLineRendererSelected.gameObject.name))
                        _currentLineRenderer.enabled = false;
                }
                else
                {
                    _currentLineRenderer.enabled = false;
                }

                _currentLineRenderer = newLineRenderer;
                newLineRenderer.enabled = true;
            }
            else
            {
                if (_currentLineRenderer.IsNotNull()) return;
                _currentLineRenderer = newLineRenderer;
                _currentLineRenderer.enabled = true;
            }
        }

        private void ShowHitSelected()
        {
            var newLineRenderer = _hit.collider.GetComponent<LineRenderer>();
            if (_currentLineRendererSelected.IsNotNull() &&
                _currentLineRendererSelected.gameObject.name.NotEquals(newLineRenderer.gameObject.name))
            {
                _currentLineRendererSelected.enabled = false;
                _currentLineRendererSelected = newLineRenderer;
                newLineRenderer.enabled = true;
            }
            else
            {
                if (_currentLineRendererSelected.IsNotNull()) return;
                _currentLineRendererSelected = newLineRenderer;
                _currentLineRendererSelected.enabled = true;
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