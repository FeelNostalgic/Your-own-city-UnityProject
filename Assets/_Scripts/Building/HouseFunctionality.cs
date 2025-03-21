using System;
using Residents;
using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Buildings
{
    public class HouseFunctionality : Building
    {
        #region Inspector Variables

        [SerializeField] private Light[] lights;
        [Range(1, 50)] [SerializeField] private int minSecondsBetweenHabitantes;
        [Range(2, 100)] [SerializeField] private int maxSecondsBetweenHabitantes;

        #endregion

        #region Public Variables

        public int Inhabitants { get; private set; }

        public int Level { get; private set; }

        public int CurrentGoldPerSecond { get; private set; }

        public int CostsPerSecond { get; private set; }

        public float Multiplier { get; private set; }

        public int MaxInhabitants { get; private set; }

        public int LevelPrice { get; private set; }

        public event Action OnResidentUpdate;
        public event Action OnLevelUpdate;

        #endregion

        #region Private Variables

        private int _currentGoldInHand;
        private int _currentLight;
        private int _secondsBetweenSpawns;
        private float _timerToSpawnHabitante;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _timerToSpawnHabitante = Random.Range(1.5f, 3.2f);
            _currentLight = 0;
            Inhabitants = 0;
            Level = 1;
            Multiplier = 1;
            CurrentGoldPerSecond = 0;
            MaxInhabitants = 1;
            LevelPrice = (int)(BuildManager.Instance.HousePrice * 2.5f);
            BuildingsManager.Instance.AddHouse();
        }

        private void Update()
        {
            //Each second genere gold
            if (Time.frameCount % Application.targetFrameRate == 0)
            {
                GenerateGold();
            }

            if (MaxInhabitants > Inhabitants) // Is there space for a new resident?
            {
                GenerateResident();
            }
        }

        #endregion

        #region Public Methods

        public void AddNewResident(HabitanteStat h)
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.newResident);
            if (Inhabitants > 0)
                ResourcesManager.Instance.AddGoldPerSecond(-CurrentGoldPerSecond * Multiplier);
            Inhabitants++;
            CurrentGoldPerSecond += h.BaseGold;
            Multiplier += h.MultiplicadorGold;
            ResourcesManager.Instance.AddGoldPerSecond(CurrentGoldPerSecond * Multiplier);
            CostsPerSecond += h.CostsPerSecond;
            UpdateLight(h.EmissionColor);
            //BuildingsManager.Instance.RemoveCasa(transform.parent.gameObject);
            ResourcesManager.Instance.AddResident(1);
            ResourcesManager.Instance.AddCosts(h.CostsPerSecond);
            OnResidentUpdate?.Invoke();
        }

        public void LevelUp()
        {
            if (ResourcesManager.Instance.CurrentGold > LevelPrice)
            {
                AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.levelUp);
                ResourcesManager.Instance.AddGold(-LevelPrice);
                Level++;
                MaxInhabitants++;
                LevelPrice *= 2;
                if (Inhabitants > 0)
                    ResourcesManager.Instance.AddGoldPerSecond((-CurrentGoldPerSecond * Multiplier));
                Multiplier += 0.1f;
                ResourcesManager.Instance.AddGoldPerSecond((CurrentGoldPerSecond * Multiplier));
                ResourcesManager.Instance.AddCosts((int)(CostsPerSecond * 0.2f));
                CostsPerSecond = (int)(CostsPerSecond * 1.2f);
                OnLevelUpdate?.Invoke();
            }
            else
            {
                UIManagerInGame.Instance.ShowNotEnoughGoldFeedback();
            }
        }

        public void UpgradeMultiplier(float m)
        {
            if (Inhabitants > 0)
                ResourcesManager.Instance.AddGoldPerSecond((-CurrentGoldPerSecond * Multiplier));
            Multiplier += m;
            ResourcesManager.Instance.AddGoldPerSecond((CurrentGoldPerSecond * Multiplier));
        }

        public override string ToString()
        {
            return
                $"{gameObject.name}: Level {Level}, Inhabitants: {Inhabitants}, Current Gold Per Second: {CurrentGoldPerSecond}, Max Inhabitants: {MaxInhabitants}, Level Price: {LevelPrice}, Costs per second: {CostsPerSecond}";
        }

        public override void Demolish()
        {
            ResourcesManager.Instance.AddGold((int)(BuildManager.Instance.HousePrice * 0.8f * Level));
            ResourcesManager.Instance.AddGoldPerSecond((-CurrentGoldPerSecond * Multiplier));
            ResourcesManager.Instance.AddCosts(-CostsPerSecond);
            ResourcesManager.Instance.AddResident(-Inhabitants);
            BuildingsManager.Instance.RemoveCasa(MapManager.Instance.GetTile(transform.parent.position));
            UIManagerInGame.Instance.DisableAllHUDExceptBuildPanel();
            Destroy(gameObject);
        }

        #endregion

        #region Private Methods

        private void GenerateResident()
        {
            if (_timerToSpawnHabitante < 0)
            {
                InhabitantsManager.Instance.SummonHabitante(transform.gameObject);
                _secondsBetweenSpawns = Random.Range(minSecondsBetweenHabitantes, maxSecondsBetweenHabitantes);
                _timerToSpawnHabitante = _secondsBetweenSpawns;
            }
            else
            {
                _timerToSpawnHabitante -= Time.deltaTime;
            }
        }

        private void GenerateGold()
        {
            if (Inhabitants > 0)
            {
                ResourcesManager.Instance.AddGold((int)(CurrentGoldPerSecond * Multiplier));
            }
        }

        private void UpdateLight(Color c)
        {
            var l = lights[_currentLight];
            _currentLight++;
            l.color = c;
            l.enabled = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Habitante")) return;
            if (MaxInhabitants > Inhabitants)
                AddNewResident(other.GetComponent<HabitanteStat>()); //If there is room for residente
            Destroy(other.gameObject);
        }

        #endregion
    }
}