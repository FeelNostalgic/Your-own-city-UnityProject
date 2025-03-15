using Residents;
using Managers;
using UnityEngine;

namespace Buildings
{
    public class CasaFunctionality : MonoBehaviour
    {
        #region Inspector Variables

        [SerializeField] private Light[] Lights;
        [Range(1, 50)] [SerializeField] private int MinSecondsBetweenHabitantes;
        [Range(2, 100)] [SerializeField] private int MaxSecondsBetweenHabitantes;

        #endregion

        #region Public Variables

        public int Habitantes => _currentHabitantes;
        public int Level => _currentLevel;
        public int CurrentGoldPorSegundo => _goldPorSegundo;
        public int GastosPorSegundo => _gastosPorSegundo;
        public float Multiplicador => _currentMultiplicador;
        public int MaxHabitantes => _maxHabitantes;
        public int CosteNivel => _costeNivel;

        #endregion

        #region Private Variables

        private int _currentHabitantes; //Maximo 3
        private int _maxHabitantes;
        private int _currentLevel; //Maximo 3
        private int _currentGoldInHand;
        private int _gastosPorSegundo;
        private float _currentMultiplicador;
        private int _goldPorSegundo;
        private int _costeNivel;
        private int _currentLight;
        private int _secondsBetweenSpawns;
        private float _timerToSpawnHabitante;

        #endregion

        #region Unity Methods

        private void Start()
        {
            _timerToSpawnHabitante = Random.Range(1.5f, 3.2f);
            _currentLight = 0;
            _currentHabitantes = 0;
            _currentLevel = 1;
            _currentMultiplicador = 1;
            _goldPorSegundo = 0;
            _maxHabitantes = 1;
            _costeNivel = (int)(BuildManager.Instance.HousePrice * 2.5f);
            BuildingsManager.Instance.AddCasa();
        }

        private void Update()
        {
            //Cada segundo se genera oro
            if (Time.frameCount % Application.targetFrameRate == 0)
            {
                generaGold();
            }

            if (_maxHabitantes > _currentHabitantes) //Hay espacio para habitantes
            {
                generaHabitante();
            }
        }

        #endregion

        #region Public Methods

        public void addNuevoHabitante(HabitanteStat h)
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.newHabitante);
            if (_currentHabitantes > 0)
                ResourcesManager.Instance.AddGoldPorSegundo((-_goldPorSegundo * _currentMultiplicador));
            _currentHabitantes++;
            _goldPorSegundo += h.BaseGold;
            _currentMultiplicador += h.MultiplicadorGold;
            ResourcesManager.Instance.AddGoldPorSegundo((_goldPorSegundo * _currentMultiplicador));
            _gastosPorSegundo += h.GastosPorSegundo;
            UpdateLight(h.EmissionColor);
            //BuildingsManager.Instance.RemoveCasa(transform.parent.gameObject);
            ResourcesManager.Instance.AddHabitante(1);
            UIManager.Instance.UpdateCasaHabitantes(this, _currentHabitantes, _currentMultiplicador, _gastosPorSegundo,
                (int)(_goldPorSegundo * _currentMultiplicador));
            ResourcesManager.Instance.AddGastos(h.GastosPorSegundo);
        }

        public void SubirNivel()
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.buttonClick);
            if (ResourcesManager.Instance.CurrentGold > _costeNivel)
            {
                AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.levelUp);
                ResourcesManager.Instance.AddGold(-_costeNivel);
                _currentLevel++;
                _maxHabitantes++;
                _costeNivel *= 2;
                if (_currentHabitantes > 0)
                    ResourcesManager.Instance.AddGoldPorSegundo((-_goldPorSegundo * _currentMultiplicador));
                _currentMultiplicador += 0.1f;
                ResourcesManager.Instance.AddGoldPorSegundo((_goldPorSegundo * _currentMultiplicador));
                ResourcesManager.Instance.AddGastos((int)(_gastosPorSegundo * 0.2f));
                _gastosPorSegundo = (int)(_gastosPorSegundo * 1.2f);
                UIManager.Instance.UpdateCasaNivel(_currentLevel, _costeNivel, _maxHabitantes, _currentMultiplicador,
                    _gastosPorSegundo, (int)(_goldPorSegundo * _currentMultiplicador));
            }
            else
            {
                UIManager.Instance.UpdateInfoGeneral("¡Gold insuficiente!");
            }
        }

        public void Demoler()
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.buttonClick);
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.detroyBuilding);
            ResourcesManager.Instance.AddGold((int)(BuildManager.Instance.HousePrice * 0.8f * _currentLevel));
            ResourcesManager.Instance.AddGoldPorSegundo((-_goldPorSegundo * _currentMultiplicador));
            ResourcesManager.Instance.AddGastos(-_gastosPorSegundo);
            ResourcesManager.Instance.AddHabitante(-_currentHabitantes);
            GetComponentInParent<BuildType>().Type = BuildManager._building.none;
            BuildingsManager.Instance.RemoveCasa(transform.parent.gameObject);
            UIManager.Instance.DisableAllPanels();
            Destroy(gameObject);
        }

        public void MejorarMultiplicador(float m)
        {
            if (_currentHabitantes > 0)
                ResourcesManager.Instance.AddGoldPorSegundo((-_goldPorSegundo * _currentMultiplicador));
            _currentMultiplicador += m;
            ResourcesManager.Instance.AddGoldPorSegundo((_goldPorSegundo * _currentMultiplicador));
        }

        #endregion

        #region Private Methods

        private void generaHabitante()
        {
            if (_timerToSpawnHabitante < 0)
            {
                ResidentsManager.Instance.SummonHabitante(transform.gameObject);
                _secondsBetweenSpawns = Random.Range(MinSecondsBetweenHabitantes, MaxSecondsBetweenHabitantes);
                _timerToSpawnHabitante = _secondsBetweenSpawns;
            }
            else
            {
                _timerToSpawnHabitante -= Time.deltaTime;
            }
        }

        private void generaGold()
        {
            if (_currentHabitantes > 0)
            {
                ResourcesManager.Instance.AddGold((int)(_goldPorSegundo * _currentMultiplicador));
            }
        }

        private void UpdateLight(Color c)
        {
            var l = Lights[_currentLight];
            _currentLight++;
            l.color = c;
            l.enabled = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Habitante"))
            {
                if (MaxHabitantes > _currentHabitantes)
                    addNuevoHabitante(other.GetComponent<HabitanteStat>()); //Hay espacio para el habitante
                Destroy(other.gameObject);
            }
        }

        #endregion
    }
}