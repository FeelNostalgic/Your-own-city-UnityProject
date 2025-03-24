using System;
using Residents;
using Managers;
using UnityEngine;

namespace Buildings
{
    public class HouseFunctionality : Building
    {
        #region Inspector Variables

        [SerializeField] private Light[] lights;

        #endregion

        #region Public Variables

        public int Inhabitants { get; private set; }
        public int Level { get; private set; }
        public int CurrentGoldPerSecond { get; private set; }
        public int CostsPerSecond { get; private set; }
        public float Multiplier { get; private set; }
        public int MaxInhabitants { get; private set; }
        public int LevelPrice { get; private set; }

        public event Action OnDemolish;
        public event Action OnResidentUpdate;
        public event Action OnLevelUpdate;

        #endregion

        #region Private Variables

        private int _currentGoldInHand;
        private int _currentLight;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _currentLight = 0;
            Inhabitants = 0;
            Level = 1;
            Multiplier = 1;
            CurrentGoldPerSecond = 0;
            MaxInhabitants = 1;
            LevelPrice = (int)(BuildManager.Instance.HousePrice * 2.5f);
            BuildingsManager.Instance.AddHouse();
            InhabitantsManager.AddAvailableHouse(this);
        }

        private void Update()
        {
            //Each second generate gold
            if (Time.frameCount % Application.targetFrameRate == 0)
            {
                GenerateGold();
            }
        }

        #endregion

        #region Public Methods
        
        public void LevelUp()
        {
            if (ResourcesManager.CurrentGold > LevelPrice)
            {
                AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.levelUp);
                ResourcesManager.AddGold(-LevelPrice);
                Level++;
                MaxInhabitants++;
                LevelPrice *= 2;
                if (Inhabitants > 0)
                    ResourcesManager.AddGoldPerSecond((-CurrentGoldPerSecond * Multiplier));
                Multiplier += 0.1f;
                ResourcesManager.AddGoldPerSecond((CurrentGoldPerSecond * Multiplier));
                ResourcesManager.AddCosts((int)(CostsPerSecond * 0.2f));
                CostsPerSecond = (int)(CostsPerSecond * 1.2f);
                InhabitantsManager.AddAvailableHouse(this);
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
                ResourcesManager.AddGoldPerSecond((-CurrentGoldPerSecond * Multiplier));
            Multiplier += m;
            ResourcesManager.AddGoldPerSecond((CurrentGoldPerSecond * Multiplier));
        }

        public override string ToString()
        {
            return
                $"{gameObject.name}: Level {Level}, Inhabitants: {Inhabitants}, Current Gold Per Second: {CurrentGoldPerSecond}, Max Inhabitants: {MaxInhabitants}, Level Price: {LevelPrice}, Costs per second: {CostsPerSecond}";
        }

        public override void Demolish()
        {
            ResourcesManager.AddGold((int)(BuildManager.Instance.HousePrice * 0.8f * Level));
            ResourcesManager.AddGoldPerSecond((-CurrentGoldPerSecond * Multiplier));
            ResourcesManager.AddCosts(-CostsPerSecond);
            ResourcesManager.AddResident(-Inhabitants);
            BuildingsManager.Instance.RemoveCasa(MapManager.Instance.GetTile(transform.parent.position));
            UIManagerInGame.Instance.DisableAllHUDExceptBuildPanel();
            OnDemolish?.Invoke();
            Destroy(gameObject);
        }
        
        #endregion

        #region Private Methods
        
        private void AddNewResident(ResidentStat h)
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.newResident);
            if (Inhabitants > 0)
                ResourcesManager.AddGoldPerSecond(-CurrentGoldPerSecond * Multiplier);
            Inhabitants++;
            CurrentGoldPerSecond += h.BaseGold;
            Multiplier += h.MultiplicadorGold;
            ResourcesManager.AddGoldPerSecond(CurrentGoldPerSecond * Multiplier);
            CostsPerSecond += h.CostsPerSecond;
            UpdateLight(h.EmissionColor);
            //BuildingsManager.Instance.RemoveCasa(transform.parent.gameObject);
            ResourcesManager.AddResident(1);
            ResourcesManager.AddCosts(h.CostsPerSecond);
            OnResidentUpdate?.Invoke();
        }
        
        private void GenerateGold()
        {
            if (Inhabitants > 0)
            {
                ResourcesManager.AddGold((int)(CurrentGoldPerSecond * Multiplier));
            }
        }

        private void UpdateLight(Color c)
        {
            var l = lights[_currentLight];
            _currentLight++;
            l.color = c;
            l.enabled = true;
        }
        
        private bool IsThereSpaceForResident()
        {
            return MaxInhabitants > Inhabitants;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Resident")) return;
            if (MaxInhabitants > Inhabitants)
                AddNewResident(other.GetComponent<ResidentStat>()); //If there is room for residente
            Destroy(other.gameObject);
        }

        #endregion
    }
}