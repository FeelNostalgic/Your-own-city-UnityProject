using System;
using System.Collections;
using Buildings;
using Commons;
using DG.Tweening;
using UI;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using Utilities;

namespace Managers
{
    [DefaultExecutionOrder(-999)]
    public class UIManagerInGame : MonoBehaviourSinglenton<UIManagerInGame>
    {
        #region Inspector Variables

        [Header("Transition")] [SerializeField]
        private Image blackScreen;

        [Header("Pause panels")] [SerializeField]
        private GameObject pausePanel;

        [SerializeField] private GameObject optionsPanel;
        [SerializeField] private GameObject controlsPanel;
        [SerializeField] private GameObject objectivesPanel;

        [Header("Pause panels buttons")] [SerializeField]
        private Button resumeButton;

        [SerializeField] private Button optionsButton;
        [SerializeField] private Button controlsButton;
        [SerializeField] private Button objectivesButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private Button controlsBackButton;
        [SerializeField] private Button objectivesBackButton;

        [Header("FeedbackStrings")] [SerializeField]
        private LocalizedString roadNotConnectedToRoadFeedback;

        [SerializeField] private LocalizedString roadNotConnectedToBuildingFeedback;
        [SerializeField] private LocalizedString notEnoughGoldFeedback;

        [Header("Game Over Panel")] [SerializeField]
        private GameObject gameOverPanel;

        [Header("General Info")] [SerializeField]
        private GeneralInfoController generalInfoPanel;

        [Header("Build")] [SerializeField] private UIPanel buildPanel;

        [Header("Road")] [SerializeField] private UIPanel roadPanel;

        [Header("House")] [SerializeField] private UIPanel housePanel;

        [Header("Info Multiplier")] [SerializeField]
        private UIPanel infoMultiplierPanel;

        #endregion

        #region Public Variables

        public bool IsAPanelActive { get; private set; }

        private enum UIPanels
        {
            none = 0,
            pausePanel,
            optionsPanel,
            controlsPanel,
            objectivesPanel,
            gameOverPanel
        }

        #endregion

        #region Private Variables

        private float _currentTime;
        private MultiplierBuildingFunctionality _currentMultiplierBuilding;

        private HUDPanels _currentHUDPanel;
        private HUDPanels _lastHUDPanel;
        private UIPanels _lastUIPanel;
        private UIPanels _currentUIPanel;
        private GameObject _currentActiveUIPanel;
        private UIPanel _currentActiveHUDPanel;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            blackScreen.DOFade(0f, 1f).SetEase(Ease.InSine)
                .OnPlay(() =>
                {
                    LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[GameManager.SelectedLocale];
                    InitializeGame();
                    blackScreen.gameObject.SetActive(true);
                })
                .Play();
        }

        #endregion

        #region Public Methods

        public void BackPanel()
        {
            PlayClickSound();
            ChangeHUDPanel(_lastHUDPanel);
        }

        public void ShowInfoPanel(TileFunctionality tile)
        {
            switch (tile.BuildingType)
            {
                case BuildingType.house:
                    housePanel.ConfigurePanel((HouseFunctionality)tile.Building);
                    break;
                case BuildingType.playground:
                case BuildingType.hospital:
                case BuildingType.police:
                    infoMultiplierPanel.ConfigurePanel((MultiplierBuildingFunctionality)tile.Building);
                    break;
                case BuildingType.road:
                    roadPanel.ConfigurePanel((RoadFunctionality)tile.Building);
                    break;
                default:
                case BuildingType.none:
                    throw new ArgumentOutOfRangeException(nameof(tile.BuildingType), tile.BuildingType, null);
            }
        }

        public void DisableAllHUDExceptBuildPanel()
        {
            if (_currentHUDPanel == HUDPanels.buildPanel) return;
            roadPanel.HidePanel();
            housePanel.HidePanel();
            infoMultiplierPanel.HidePanel();
        }

        public void DisableAllPanels()
        {
            infoMultiplierPanel.HidePanel();
            buildPanel.HidePanel();
            roadPanel.HidePanel();
            housePanel.HidePanel();
            pausePanel.SetActive(false);
            optionsPanel.SetActive(false);
            controlsPanel.SetActive(false);
            objectivesPanel.SetActive(false);

            _lastUIPanel = UIPanels.none;
            _lastHUDPanel = HUDPanels.none;

            IsAPanelActive = false;
        }

        public void ShowRoadNotConnectedToRoadFeedback()
        {
            UpdateFeedback(roadNotConnectedToRoadFeedback.GetLocalizedString());
        }

        public void ShowRoadNotConnectedToBuildingFeedback()
        {
            UpdateFeedback(roadNotConnectedToBuildingFeedback.GetLocalizedString());
        }

        public void ShowNotEnoughGoldFeedback()
        {
            UpdateFeedback(notEnoughGoldFeedback.GetLocalizedString());
        }

        public void ShowFinalPanel(bool value)
        {
            DisableAllPanels();
            IsAPanelActive = value;
            generalInfoPanel.gameObject.SetActive(!value);
            gameOverPanel.SetActive(value);
        }

        #region Button Functionalities

        public void PauseGame()
        {
            GameManager.Instance.ChangeState(GameState.Paused);
            ChangeUIPanel(UIPanels.pausePanel);
            Time.timeScale = 0;
        }

        public void UnpauseGame()
        {
            _currentActiveUIPanel.SetActive(false);
            Time.timeScale = 1;
            GameManager.Instance.ChangeState(GameState.Playing);
        }

        public static void ExitButton()
        {
            PlayClickSound();
            GameManager.Instance.ChangeState(GameState.NotStarted);
            MySceneManager.LoadScene(MySceneManager.Scenes.MainMenuScene);
        }

        public void RestartButton()
        {
            PlayClickSound();
            MapManager.Instance.DestroyAllMap();
            BuildingsManager.Instance.RestartBuildings();
            ResourcesManager.Instance.RestartAllInfo();
            InitializeGame();
            ShowFinalPanel(false);
            GameManager.Instance.ChangeState(GameState.Starting);
        }

        #endregion

        #endregion

        #region Private Methods

        private void InitializeGame()
        {
            MapManager.Instance.InitializeMap();
            DisableAllPanels();
            InitializeButtonListeners();
            IsAPanelActive = false;
            Time.timeScale = 1;
            GameManager.Instance.ChangeState(GameState.Playing);
            AudioManager.Instance.PlayMainSound();
        }

        private void InitializeButtonListeners()
        {
            resumeButton.onClick.AddListener(UnpauseGame);
            optionsButton.onClick.AddListener(() =>
            {
                PlayClickSound();
                ChangeUIPanel(UIPanels.optionsPanel);
            });
            controlsButton.onClick.AddListener(() =>
            {
                PlayClickSound();
                ChangeUIPanel(UIPanels.controlsPanel);
            });
            objectivesButton.onClick.AddListener(() =>
            {
                PlayClickSound();
                ChangeUIPanel(UIPanels.objectivesPanel);
            });
            exitButton.onClick.AddListener(ExitButton);
            controlsBackButton.onClick.AddListener(BackPanel);
            objectivesBackButton.onClick.AddListener(BackPanel);
        }

        private void ChangeUIPanel(UIPanels newPanel)
        {
            _lastUIPanel = _currentUIPanel;
            _currentUIPanel = newPanel;
            switch (newPanel)
            {
                case UIPanels.gameOverPanel:
                    throw new NotImplementedException();
                case UIPanels.pausePanel:
                    if (_currentActiveUIPanel) _currentActiveUIPanel.SetActive(false);
                    SetPanelActive(pausePanel);
                    break;
                case UIPanels.optionsPanel:
                    if (_currentActiveUIPanel) _currentActiveUIPanel.SetActive(false);
                    SetPanelActive(optionsPanel);
                    break;
                case UIPanels.controlsPanel:
                    if (_currentActiveUIPanel) _currentActiveUIPanel.SetActive(false);
                    SetPanelActive(controlsPanel);
                    break;
                case UIPanels.objectivesPanel:
                    if (_currentActiveUIPanel) _currentActiveUIPanel.SetActive(false);
                    SetPanelActive(objectivesPanel);
                    break;
                case UIPanels.none:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newPanel), newPanel, null);
            }

            Debug.Log($"Changed UI Panel from {_lastHUDPanel} to {_currentHUDPanel}");
        }

        public void ChangeHUDPanel(HUDPanels newPanel)
        {
            _lastHUDPanel = _currentHUDPanel;
            _currentHUDPanel = newPanel;
            switch (newPanel)
            {
                case HUDPanels.buildPanel:
                    if (_currentActiveHUDPanel) _currentActiveHUDPanel.HidePanel();
                    buildPanel.ShowPanel();
                    _currentActiveHUDPanel = buildPanel;
                    break;
                case HUDPanels.roadPanel:
                case HUDPanels.housePanel:
                case HUDPanels.multiplierPanel:
                    StartCoroutine(HideLastPanelAndWaitToShowNew());
                    break;
                case HUDPanels.none:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newPanel), newPanel, null);
            }

            Debug.Log($"Changed HUD Panel from {_lastHUDPanel} to {_currentHUDPanel}");
        }

        private IEnumerator HideLastPanelAndWaitToShowNew()
        {
            //Hide last
            if (_currentActiveHUDPanel) _currentActiveHUDPanel.HidePanel();
            var timeToWait = _currentActiveHUDPanel ? _currentActiveHUDPanel.TimeBetweenAnimations : 0;
            yield return new WaitForSeconds(timeToWait);
            //Show current
            switch (_currentHUDPanel)
            {
                case HUDPanels.buildPanel:
                    _currentActiveHUDPanel = buildPanel;
                    break;
                case HUDPanels.roadPanel:
                    _currentActiveHUDPanel = roadPanel;
                    break;
                case HUDPanels.housePanel:
                    _currentActiveHUDPanel = housePanel;
                    break;
                case HUDPanels.multiplierPanel:
                    _currentActiveHUDPanel = infoMultiplierPanel;
                    break;
                case HUDPanels.none:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _currentActiveHUDPanel.ShowPanel();
        }

        private void UpdateFeedback(string info)
        {
            generalInfoPanel.ShowFeedback(info);
        }

        private static void PlayClickSound()
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.buttonClick);
        }

        private void SetPanelActive(GameObject newPanel)
        {
            newPanel.SetActive(true);
            _currentActiveUIPanel = newPanel;
        }

        #endregion
    }
}