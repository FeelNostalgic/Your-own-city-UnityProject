using System;
using System.Collections;
using System.Collections.Generic;
using Buildings;
using DG.Tweening;
using TMPro;
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

        [Header("House")] [SerializeField] private HousePanelController housePanel;

        [Header("InfoMultipliyer")] [SerializeField]
        private GameObject InfoMultiplierPanel;

        [SerializeField] private TMP_Text InfoMultiplierTitulo;
        [SerializeField] private TMP_Text InfoMultiplierNivel;
        [SerializeField] private TMP_Text InfoMultiplierGastorPorSegundo;
        [SerializeField] private TMP_Text InfoMultiplierMultiplicador;
        [SerializeField] private TMP_Text InfoMultiplierAreaEfecto;
        [SerializeField] private TMP_Text InfoMultiplierCasaAfectadas;
        [SerializeField] private TMP_Text InfoMultiplierDesciptionText;
        [SerializeField] private Button Button_InfoMultiplierSubirNivel;
        [SerializeField] private TMP_Text InfoMultiplierCosteNivelText;
        [SerializeField] private Button Button_InfoMultiplierDemoler;
        [SerializeField] private TMP_Text InfoMultiplierDemolerText;

        [Header("Parque")] [TextArea(2, 5)] [SerializeField]
        private string ParqueDescription;

        [Header("Hospital")] [TextArea(2, 5)] [SerializeField]
        private string HospitalDescription;

        [Header("Policia")] [TextArea(2, 5)] [SerializeField]
        private string PoliciaDescription;

        #endregion

        #region Public Variables

        public bool IsAPanelActive { get; private set; }

        public enum HUDPanels
        {
            none = 0,
            buildPanel,
            roadPanel,
            housePanel,
            playgroundPanel,
            hospitalPanel,
            policePanel
        }

        public enum UIPanels
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
        private ParqueFunctionality _currentPlayground;
        private HospitalFunctionality _currentHospital;
        private PoliciaFunctionality _currentPolice;

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

        private void Update()
        {
            //UpdateTime();
        }

        #endregion

        #region Public Methods

        public void BackPanel()
        {
            PlayClickSound();
            ChangeHUDPanel(_lastHUDPanel);
        }

        public void ShowInfoPanel(BuildManager.BuildingType type, GameObject selectedBuilding)
        {
            switch (type)
            {
                case BuildManager.BuildingType.house:
                    HideAllAreas();
                    housePanel.ConfigureHousePanel(selectedBuilding.GetComponentInChildren<HouseFunctionality>());
                    break;

                case BuildManager.BuildingType.playground:
                    HideAllAreas();
                    _currentPlayground = selectedBuilding.GetComponentInChildren<ParqueFunctionality>();
                    _currentPlayground.ShowArea();
                    ConfigureInfoMultiplierPanel(_currentPlayground.Level, _currentPlayground.GastosPorSegundo,
                        _currentPlayground.Multiplicador, _currentPlayground.AreaEfecto, _currentPlayground.CasaAfectadas,
                        ParqueDescription, _currentPlayground.CosteNivel, parque: _currentPlayground);
                    break;

                case BuildManager.BuildingType.road:
                    HideAllAreas();
                    MapManager.Instance.RoadToDestroy = selectedBuilding.gameObject;
                    ChangeHUDPanel(HUDPanels.roadPanel);
                    break;

                case BuildManager.BuildingType.hospital:
                    HideAllAreas();
                    _currentHospital = selectedBuilding.GetComponentInChildren<HospitalFunctionality>();
                    _currentHospital.ShowArea();
                    ConfigureInfoMultiplierPanel(_currentHospital.Level, _currentHospital.GastosPorSegundo,
                        _currentHospital.Multiplicador, _currentHospital.AreaEfecto, _currentHospital.CasaAfectadas,
                        HospitalDescription, _currentHospital.CosteNivel, hospital: _currentHospital);
                    break;

                case BuildManager.BuildingType.police:
                    HideAllAreas();
                    _currentPolice = selectedBuilding.GetComponentInChildren<PoliciaFunctionality>();
                    _currentPolice.ShowArea();
                    ConfigureInfoMultiplierPanel(_currentPolice.Level, _currentPolice.GastosPorSegundo,
                        _currentPolice.Multiplicador, _currentPolice.AreaEfecto, _currentPolice.CasaAfectadas,
                        PoliciaDescription, _currentPolice.CosteNivel, policia: _currentPolice);
                    break;
                case BuildManager.BuildingType.none:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public void UpdateInfoMultiplierNivel(int currentLevel, int gastosPorSegundo, float currentMultiplicador, int areaEfecto, int coste, int casasAfectadas, float price)
        {
            InfoMultiplierNivel.text = "NIVEL " + currentLevel;
            InfoMultiplierGastorPorSegundo.text = gastosPorSegundo.ToString();
            InfoMultiplierMultiplicador.text = "X" + currentMultiplicador.ToString().Replace(",", ".");
            InfoMultiplierAreaEfecto.text = areaEfecto.ToString();
            InfoMultiplierCasaAfectadas.text = casasAfectadas.ToString();
            if (currentLevel > 2)
            {
                InfoMultiplierCosteNivelText.text = "NIVEL MAX";
                Button_InfoMultiplierSubirNivel.interactable = false;
            }
            else InfoMultiplierCosteNivelText.text = "SUBIR NIVEL X " + coste;

            InfoMultiplierDemolerText.text = "DEMOLER (+" + (int)(price * 0.8 * currentLevel) + ")";
        }

        public void DisableAllHUDExceptBuildPanel()
        {
            if (_currentHUDPanel == HUDPanels.buildPanel) return;
            roadPanel.HidePanel();
            housePanel.HidePanel();
            // InfoMultiplierPanel.SetActive(false);
        }

        public void DisableAllPanels()
        {
            // TODO
            // InfoMultiplierPanel.SetActive(false);
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

        public void HideAllAreas()
        {
            if (_currentPolice != null) _currentPolice.HideArea();
            if (_currentHospital != null) _currentHospital.HideArea();
            if (_currentPlayground != null) _currentPlayground.HideArea();
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
                    StartCoroutine(HideLastPanelAndWaitToShowNew());
                    break;
                case HUDPanels.playgroundPanel:
                case HUDPanels.hospitalPanel:
                case HUDPanels.policePanel:
                    throw new NotImplementedException();
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
                case HUDPanels.playgroundPanel:
                case HUDPanels.hospitalPanel:
                case HUDPanels.policePanel:
                    throw new NotImplementedException();
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

        private void ConfigureInfoMultiplierPanel(int level, int gastosPorSegundo, float multiplicador, int areaEfecto,
            int casasAfectadas, string descriptionText, int costeNivel, ParqueFunctionality parque = null,
            HospitalFunctionality hospital = null, PoliciaFunctionality policia = null)
        {
            if (parque != null) InfoMultiplierTitulo.text = "PARQUE";
            if (hospital != null) InfoMultiplierTitulo.text = "HOSPITAL";
            if (policia != null) InfoMultiplierTitulo.text = "POLICIA";

            InfoMultiplierNivel.text = "NIVEL " + level;
            InfoMultiplierGastorPorSegundo.text = gastosPorSegundo.ToString();
            InfoMultiplierMultiplicador.text = "X" + multiplicador.ToString().Replace(",", ".");
            InfoMultiplierAreaEfecto.text = areaEfecto.ToString();
            InfoMultiplierCasaAfectadas.text = casasAfectadas.ToString();

            if (level > 2)
            {
                InfoMultiplierCosteNivelText.text = "NIVEL MAX";
                Button_InfoMultiplierSubirNivel.interactable = false;
            }
            else
            {
                Button_InfoMultiplierSubirNivel.interactable = true;
                InfoMultiplierCosteNivelText.text = "SUBIR NIVEL X " + costeNivel;
            }

            InfoMultiplierDesciptionText.text = descriptionText;

            if (parque != null) ConfigureInfoMultiplierButtons(parque: parque);
            if (hospital != null) ConfigureInfoMultiplierButtons(hospital: hospital);
            if (policia != null) ConfigureInfoMultiplierButtons(policia: policia);

            IsAPanelActive = true;
            InfoMultiplierPanel.SetActive(true);
        }

        private void ConfigureInfoMultiplierButtons(ParqueFunctionality parque = null,
            HospitalFunctionality hospital = null, PoliciaFunctionality policia = null)
        {
            if (parque != null)
            {
                Button_InfoMultiplierSubirNivel.onClick.RemoveAllListeners();
                Button_InfoMultiplierSubirNivel.onClick.AddListener(delegate { parque.SubirNivel(); });

                InfoMultiplierDemolerText.text =
                    "DEMOLER (+" + (int)(BuildManager.Instance.PlaygroundPrice * 0.8 * parque.Level) + ")";
                Button_InfoMultiplierDemoler.onClick.RemoveAllListeners();
                Button_InfoMultiplierDemoler.onClick.AddListener(delegate { parque.Demoler(); });
                return;
            }

            if (hospital != null)
            {
                Button_InfoMultiplierSubirNivel.onClick.RemoveAllListeners();
                Button_InfoMultiplierSubirNivel.onClick.AddListener(delegate { hospital.SubirNivel(); });

                InfoMultiplierDemolerText.text =
                    "DEMOLER (+" + (int)(BuildManager.Instance.HospitalPrice * 0.8 * hospital.Level) + ")";
                Button_InfoMultiplierDemoler.onClick.RemoveAllListeners();
                Button_InfoMultiplierDemoler.onClick.AddListener(delegate { hospital.Demoler(); });
                return;
            }

            if (policia != null)
            {
                Button_InfoMultiplierSubirNivel.onClick.RemoveAllListeners();
                Button_InfoMultiplierSubirNivel.onClick.AddListener(delegate { policia.SubirNivel(); });

                InfoMultiplierDemolerText.text =
                    "DEMOLER (+" + (int)(BuildManager.Instance.PolicePrice * 0.8 * policia.Level) + ")";
                Button_InfoMultiplierDemoler.onClick.RemoveAllListeners();
                Button_InfoMultiplierDemoler.onClick.AddListener(delegate { policia.Demoler(); });
            }
        }

        #endregion
    }
}