using System.Collections;
using Buildings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Managers
{
    public class UIManager : MonoBehaviourSinglenton<UIManager>
    {
        #region Inspector Variables
        
        [Header("MenuPausa")]
        [SerializeField] private GameObject ButtonPausa;

        [Header("PanelFinal")]
        [SerializeField] private GameObject FinalPanel;
        
        [Header("Principal")]
        [SerializeField] private GameObject PanelGeneral;
        [SerializeField] private TMP_Text Tiempo;
        [SerializeField] private TMP_Text Gold;
        [SerializeField] private TMP_Text Habitantes;
        [SerializeField] private TMP_Text Gastos;
        [SerializeField] private TMP_Text GoldPorSegundo;
        [SerializeField] private TMP_Text InfoGeneral;
        [SerializeField] private float TiempoInfoActiva;

        [Header("Buildings")] [SerializeField] private GameObject BuildPanel;
        [SerializeField] private TMP_Text CarreteraPrice;
        [SerializeField] private TMP_Text CasaPrice;
        [SerializeField] private TMP_Text ParquePrice;
        [SerializeField] private TMP_Text HospitalPrice;
        [SerializeField] private TMP_Text PoliciaPrice;

        [Header("Carretera")] [SerializeField] private GameObject CarreteraPanel;
        [SerializeField] private Button Button_CarreteraDemoler;
        [SerializeField] private TMP_Text CarreteraDemolerText;

        [Header("Casa")] [SerializeField] private GameObject CasaPanel;
        [SerializeField] private TMP_Text CasaNivel;
        [SerializeField] private TMP_Text CasaHabitantes;
        [SerializeField] private TMP_Text CasaMaxHabitantes;
        [SerializeField] private TMP_Text CasaGastosPorSegundo;
        [SerializeField] private TMP_Text CasaMultiplier;
        [SerializeField] private TMP_Text CasaGoldPorSegundo;
        [SerializeField] private TMP_Text CasaCosteNivel;
        [SerializeField] private Button Button_CasaSubirNivel;
        [SerializeField] private Button Button_CasaDemoler;
        [SerializeField] private TMP_Text CasaDemolerText;

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
        private CasaFunctionality _currentCasa;
        private ParqueFunctionality _currentParque;
        private HospitalFunctionality _currentHospital;
        private PoliciaFunctionality _currentPolicia;
        private bool _isAPanelActive;
        
        #endregion

        #region Unity Methods

        private void Awake()
        {
            Time.timeScale = 0;
        }

        private void Start()
        {
            ShowBuildPanel(false);
            UpdateBuildPanel();
            InfoGeneral.enabled = false;
            UpdateHabitantes(0);
            CarreteraDemolerText.text = "DEMOLER (+" + (int)(BuildManager.Instance.RoadPrice * 0.8) + ")";
            Button_CarreteraDemoler.onClick.AddListener(delegate { MapManager.Instance.DestroyRoad(); });
            _isAPanelActive = false;
        }

        private void Update()
        {
            //UpdateTime();
        }

        #endregion

        #region Public Methods

        public void ShowPanelInfo(BuildManager._building type, Collider currentBuild)
        {
            switch (type)
            {
                case BuildManager._building.casa:
                    HideAllAreas();
                    _currentCasa = currentBuild.GetComponentInChildren<CasaFunctionality>();
                    ConfigureCasaPanel();
                    break;

                case BuildManager._building.parque:
                    HideAllAreas();
                    _currentParque = currentBuild.GetComponentInChildren<ParqueFunctionality>();
                    _currentParque.ShowArea();
                    ConfigureInfoMultiplierPanel(_currentParque.Level, _currentParque.GastosPorSegundo,
                        _currentParque.Multiplicador, _currentParque.AreaEfecto, _currentParque.CasaAfectadas,
                        ParqueDescription, _currentParque.CosteNivel, parque: _currentParque);
                    break;

                case BuildManager._building.road:
                    HideAllAreas();
                    MapManager.Instance.RoadToDestroy = currentBuild.gameObject;
                    CarreteraPanel.SetActive(true);
                    break;

                case BuildManager._building.hospital:
                    HideAllAreas();
                    _currentHospital = currentBuild.GetComponentInChildren<HospitalFunctionality>();
                    _currentHospital.ShowArea();
                    ConfigureInfoMultiplierPanel(_currentHospital.Level, _currentHospital.GastosPorSegundo,
                        _currentHospital.Multiplicador, _currentHospital.AreaEfecto, _currentHospital.CasaAfectadas,
                        HospitalDescription, _currentHospital.CosteNivel, hospital: _currentHospital);
                    break;

                case BuildManager._building.policia:
                    HideAllAreas();
                    _currentPolicia = currentBuild.GetComponentInChildren<PoliciaFunctionality>();
                    _currentPolicia.ShowArea();
                    ConfigureInfoMultiplierPanel(_currentPolicia.Level, _currentPolicia.GastosPorSegundo,
                        _currentPolicia.Multiplicador, _currentPolicia.AreaEfecto, _currentPolicia.CasaAfectadas,
                        PoliciaDescription, _currentPolicia.CosteNivel, policia: _currentPolicia);
                    break;
            }
        }

        public void UpdateCasaHabitantes(CasaFunctionality casa, int habitantes, float multiplicador, int gastosPorSegundo, int goldPorSegundo)
        {
            if (_currentCasa != null && casa.transform.parent.name.Equals(_currentCasa.transform.parent.name))
            {
                CasaHabitantes.text = habitantes.ToString();
                CasaGastosPorSegundo.text = gastosPorSegundo.ToString();
                CasaGoldPorSegundo.text = goldPorSegundo.ToString();
                CasaMultiplier.text = "X" + multiplicador.ToString().Replace(",", ".");
            }
        }

        public void UpdateCasaNivel(int level, int coste, int maxHabitantes, float multiplicador, int gastos,
            int goldPorSegundo)
        {
            CasaNivel.text = "NIVEL " + level;
            CasaMaxHabitantes.text = maxHabitantes.ToString();
            CasaGastosPorSegundo.text = gastos.ToString();
            CasaMultiplier.text = "X" + multiplicador.ToString().Replace(",", ".");
            CasaGoldPorSegundo.text = goldPorSegundo.ToString();
            if (level > 2)
            {
                CasaCosteNivel.text = "NIVEL MAX";
                Button_CasaSubirNivel.interactable = false;
            }
            else CasaCosteNivel.text = "SUBIR NIVEL X " + coste;

            CasaDemolerText.text = "DEMOLER (+" + (int)(BuildManager.Instance.HousePrice * 0.8 * level) + ")";
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
            BuildPanel.SetActive(value);
            if (!value)
            {
                PointAndClickManager.Instance.IsEnableRaycast = true;
            }
        }

        public void DisableAllPanels()
        {
            CasaPanel.SetActive(false);
            CarreteraPanel.SetActive(false);
            InfoMultiplierPanel.SetActive(false);
            BuildPanel.SetActive(false);
            PointAndClickManager.Instance.IsEnableRaycast = true;
            _isAPanelActive = false;
        }

        public void UpdateGold(int g)
        {
            Gold.text = string.Format("{0:N0}", g);
            if (g < 0) Gold.color = Color.red;
            else Gold.color = Color.white;
        }

        public void UpdateHabitantes(int h)
        {
            Habitantes.text = h.ToString();
        }

        public void UpdateGastos(int g)
        {
            Gastos.text = g.ToString(); //string.Format("{0:N0}", g);
        }
        
        public void UpdateGoldPorSegundo(int g)
        {
            GoldPorSegundo.text = g.ToString(); //string.Format("{0:N0}", g);
        }

        public void UpdateInfoGeneral(string info)
        {
            if (!InfoGeneral.enabled) StartCoroutine(showInfoGeneral(info));
        }
        
        public void HideAllAreas()
        {
            if(_currentPolicia != null) _currentPolicia.HideArea();
            if(_currentHospital != null) _currentHospital.HideArea();
            if(_currentParque != null) _currentParque.HideArea();
        }

        public void ShowFinalPanel(bool value)
        {
            DisableAllPanels();
            _isAPanelActive = value;
            PanelGeneral.SetActive(!value);
            FinalPanel.SetActive(value);
        }

        #region ButtonFunctionalities

        public void StartGame()
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.buttonClick);
            MapManager.Instance.StartMap();
            Time.timeScale = 1;
            AudioManager.Instance.PlayMainSound();
        }
        
        public void PauseGame(bool state)
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.buttonClick);
            //InicioPanel.SetActive(state);
            PointAndClickManager.Instance.IsGamePaused = state;
            if (state) Time.timeScale = 0;
            else Time.timeScale = 1;
        }

        public void ExtiButton()
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.buttonClick);
            Application.Quit();
        }

        public void RestartButton()
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.buttonClick);
            MapManager.Instance.DestroyAll();
            BuildingsManager.Instance.RestartBuildings();
            ResourcesManager.Instance.RestartAllInfo();
            StartGame();
            ShowFinalPanel(false);
            PointAndClickManager.Instance.IsGamePaused = false;
            PointAndClickManager.Instance.IsGameOver = false;
        }
        

        #endregion

        #endregion

        #region Private Methods

        private void ConfigureCasaPanel()
        {
            CasaNivel.text = "NIVEL " + _currentCasa.Level;
            CasaHabitantes.text = _currentCasa.Habitantes.ToString();
            CasaMaxHabitantes.text = _currentCasa.MaxHabitantes.ToString();
            CasaGastosPorSegundo.text = _currentCasa.GastosPorSegundo.ToString();
            CasaMultiplier.text = "X" + _currentCasa.Multiplicador.ToString().Replace(",", ".");
            CasaGoldPorSegundo.text = ((int)(_currentCasa.CurrentGoldPorSegundo * _currentCasa.Multiplicador)).ToString();

            if (_currentCasa.Level > 2)
            {
                CasaCosteNivel.text = "NIVEL MAX";
                Button_CasaSubirNivel.interactable = false;
            }
            else
            {
                Button_CasaSubirNivel.interactable = true;
                CasaCosteNivel.text = "SUBIR NIVEL X " + _currentCasa.CosteNivel;
            }

            Button_CasaSubirNivel.onClick.RemoveAllListeners();
            Button_CasaSubirNivel.onClick.AddListener(delegate { _currentCasa.SubirNivel(); });

            CasaDemolerText.text =
                "DEMOLER (+" + (int)(BuildManager.Instance.HousePrice * 0.8 * _currentCasa.Level) + ")";
            Button_CasaDemoler.onClick.RemoveAllListeners();
            Button_CasaDemoler.onClick.AddListener(delegate { _currentCasa.Demoler(); });

            _isAPanelActive = true;
            CasaPanel.SetActive(true);
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
                    "DEMOLER (+" + (int)(BuildManager.Instance.ParquePrice * 0.8 * parque.Level) + ")";
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

        private void UpdateTime()
        {
            _currentTime += Time.deltaTime;
            var minutos = Mathf.FloorToInt(_currentTime / 60);
            var segundos = Mathf.FloorToInt(_currentTime % 60);
            Tiempo.text = string.Format("{0:00}:{1:00}", minutos, segundos);
        }

        private void UpdateBuildPanel()
        {
            CarreteraPrice.text = string.Format("{0:N0}", (int)BuildManager.Instance.RoadPrice);
            CasaPrice.text = string.Format("{0:N0}", (int)BuildManager.Instance.HousePrice);
            ParquePrice.text = string.Format("{0:N0}", (int)BuildManager.Instance.ParquePrice);
            HospitalPrice.text = string.Format("{0:N0}", (int)BuildManager.Instance.HospitalPrice);
            PoliciaPrice.text = string.Format("{0:N0}", (int)BuildManager.Instance.PolicePrice);
        }

        private IEnumerator showInfoGeneral(string info)
        {
            InfoGeneral.text = info;
            InfoGeneral.enabled = true;
            yield return new WaitForSeconds(TiempoInfoActiva);
            InfoGeneral.enabled = false;
        }

        #endregion
    }
}