using System;
using System.Collections;
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

        public bool IsAPanelActive => _isAPanelActive;

        #endregion

        #region Private Variables

        private float _currentTime;
        private ParqueFunctionality _currentPlayground;
        private HospitalFunctionality _currentHospital;
        private PoliciaFunctionality _currentPolice;
        private bool _isAPanelActive;

        private UIPanels _currentPanel = UIPanels.none;
        private UIPanels _lastPanel = UIPanels.none;
        private GameObject _currentActivePanel;

        private enum UIPanels
        {
            none = 0,
            buildPanel,
            roadPanel,
            housePanel,
            playgroundPanel,
            hospitalPanel,
            policePanel,
            pausePanel,
            optionsPanel,
            controlsPanel,
            objectivesPanel,
            gameOverPanel
        }

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
            ChangeUIPanel(_lastPanel);
        }

        public void ShowInfoPanel(BuildManager.BuildingType type, GameObject selectedBuilding)
        {
            switch (type)
            {
                case BuildManager.BuildingType.house:
                    HideAllAreas();
                    housePanel.ConfigureHousePanel(selectedBuilding.GetComponentInChildren<HouseFunctionality>());
                    ChangeUIPanel(UIPanels.housePanel);
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
                    ChangeUIPanel(UIPanels.roadPanel);
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
            }
        }

        public void UpdateInfoMultiplierNivel(int currentLevel, int gastosPorSegundo, float currentMultiplicador,
            int areaEfecto, int coste, int casasAfectadas, float price)
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

        public void ShowBuildPanel(bool value)
        {
            _isAPanelActive = value;
            if (value)
            {
                ChangeUIPanel(UIPanels.buildPanel);
            }
            else
            {
                ChangeUIPanel(UIPanels.none);
                buildPanel.HidePanel();
            }
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
            _isAPanelActive = false;
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
            _isAPanelActive = value;
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
            _currentActivePanel.SetActive(false);
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
            MapManager.Instance.DestroyAll();
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
            _isAPanelActive = false;
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
            _lastPanel = _currentPanel;
            _currentPanel = newPanel;
            switch (newPanel)
            {
                case UIPanels.buildPanel:
                    if (_lastPanel == UIPanels.buildPanel) return;
                    buildPanel.ShowPanel();
                    break;
                case UIPanels.roadPanel:
                case UIPanels.housePanel:
                    StartCoroutine(HideLastPanelAndWaitToShowNew());
                    break;
                case UIPanels.playgroundPanel:
                case UIPanels.hospitalPanel:
                case UIPanels.policePanel:
                case UIPanels.gameOverPanel:
                    throw new NotImplementedException();
                case UIPanels.pausePanel:
                    if (_currentActivePanel) _currentActivePanel.SetActive(false);
                    SetPanelActive(pausePanel);
                    break;
                case UIPanels.optionsPanel:
                    if (_currentActivePanel) _currentActivePanel.SetActive(false);
                    SetPanelActive(optionsPanel);
                    break;
                case UIPanels.controlsPanel:
                    if (_currentActivePanel) _currentActivePanel.SetActive(false);
                    SetPanelActive(controlsPanel);
                    break;
                case UIPanels.objectivesPanel:
                    if (_currentActivePanel) _currentActivePanel.SetActive(false);
                    SetPanelActive(objectivesPanel);
                    break;
                case UIPanels.none:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newPanel), newPanel, null);
            }

            Debug.Log($"Changed UI Panel from {_lastPanel} to {_currentPanel}");
        }

        private IEnumerator HideLastPanelAndWaitToShowNew()
        {
            switch (_lastPanel)
            {
                case UIPanels.buildPanel:
                    buildPanel.HidePanel();
                    yield return new WaitForSeconds(buildPanel.TimeBetweenAnimations);
                    break;
                case UIPanels.roadPanel:
                    roadPanel.HidePanel();
                    yield return new WaitForSeconds(roadPanel.TimeBetweenAnimations);
                    break;
                case UIPanels.housePanel:
                    housePanel.HidePanel();
                    yield return new WaitForSeconds(housePanel.TimeBetweenAnimations);
                    break;
                case UIPanels.playgroundPanel:
                case UIPanels.hospitalPanel:
                case UIPanels.policePanel:
                    throw new NotImplementedException();
                case UIPanels.none:
                case UIPanels.pausePanel:
                case UIPanels.optionsPanel:
                case UIPanels.controlsPanel:
                case UIPanels.objectivesPanel:
                case UIPanels.gameOverPanel:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (_currentPanel)
            {
                case UIPanels.buildPanel:
                    buildPanel.ShowPanel();
                    break;
                case UIPanels.roadPanel:
                    roadPanel.ShowPanel();
                    break;
                case UIPanels.housePanel:
                    housePanel.ShowPanel();
                    break;
                case UIPanels.playgroundPanel:
                case UIPanels.hospitalPanel:
                case UIPanels.policePanel:
                    throw new NotImplementedException();
                case UIPanels.none:
                case UIPanels.pausePanel:
                case UIPanels.optionsPanel:
                case UIPanels.controlsPanel:
                case UIPanels.objectivesPanel:
                case UIPanels.gameOverPanel:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
            _currentActivePanel = newPanel;
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

            _isAPanelActive = true;
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