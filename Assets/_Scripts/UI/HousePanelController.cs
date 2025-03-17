using System.Globalization;
using Buildings;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using Utilities;

namespace UI
{
    public class HousePanelController : UIPanel
    {
        #region Inspector Variables

        [Header("TMP texts")] [SerializeField] private TMP_Text houseLevelTMP;
        [SerializeField] private TMP_Text houseInhabitantsTMP;
        [SerializeField] private TMP_Text houseMaxInhabitantsTMP;
        [SerializeField] private TMP_Text houseCostsPerSecondTMP;
        [SerializeField] private TMP_Text houseMultiplierTMP;
        [SerializeField] private TMP_Text houseGoldPerSecondTMP;

        [Header("Buttons")] [SerializeField] private TMP_Text houseLevelUpButtonTMP;
        [SerializeField] private Button houseLevelUpButton;
        [SerializeField] private TMP_Text houseDestroyButtonTMP;
        [SerializeField] private Button houseDestroyButton;

        [Header("Localized Strings")] [SerializeField]
        private LocalizedString levelLocalizedString;

        [SerializeField] private LocalizedString maxLevelLocalizedString;
        [SerializeField] private LocalizedString levelUpLocalizedString;
        [SerializeField] private LocalizedString destroyLocalizedString;

        #endregion

        #region Public Variables

        public int DestroyPrice { get; private set; }
        public int LevelUpPrice { get; private set; }
        public int Level { get; private set; }

        #endregion

        #region Private Variables

        private HouseFunctionality _house;

        #endregion

        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();
            levelLocalizedString.Arguments = new object[] { this };
            levelUpLocalizedString.Arguments = new object[] { this };
            destroyLocalizedString.Arguments = new object[] { this };

            levelLocalizedString.StringChanged += UpdateLevelString;
            levelUpLocalizedString.StringChanged += UpdateLevelUpButtonString;
            destroyLocalizedString.StringChanged += UpdateDestroyButtonTMP;

            OwnAnimateUI.OnShowAnimationPlay += ConfigureHousePanel;
        }

        private void OnDestroy()
        {
            levelLocalizedString.StringChanged -= UpdateLevelString;
            levelUpLocalizedString.StringChanged -= UpdateLevelUpButtonString;
            destroyLocalizedString.StringChanged -= UpdateDestroyButtonTMP;
            
            OwnAnimateUI.OnShowAnimationPlay -= ConfigureHousePanel;
        }

        #endregion

        #region Public Methods

        public void ConfigureHousePanel(HouseFunctionality house)
        {
            // If hit same house when is opened just ignore
            if (OwnAnimateUI.IsOpen && _house.IsNotNull() && _house.gameObject.name.Equals(house.gameObject.name)) return;
            _house = house;
            UIManagerInGame.Instance.ChangeHUDPanel(UIManagerInGame.HUDPanels.housePanel);
        }
        
        #endregion

        #region Private Methods

        private void ConfigureHousePanel()
        {
            Level = _house.Level;
            houseInhabitantsTMP.text = _house.Inhabitants.ToString();
            houseMaxInhabitantsTMP.text = _house.MaxInhabitants.ToString();
            houseCostsPerSecondTMP.text = _house.CostsPerSecond.ToString();
            houseMultiplierTMP.text = "X" + _house.Multiplier.ToString(CultureInfo.InvariantCulture).Replace(",", ".");
            houseGoldPerSecondTMP.text = ((int)(_house.CurrentGoldPerSecond * _house.Multiplier)).ToString();

            if (_house.Level > 2)
            {
                houseLevelUpButtonTMP.text = maxLevelLocalizedString.GetLocalizedString();
                houseLevelUpButton.interactable = false;
            }
            else
            {
                houseLevelUpButton.interactable = true;
                LevelUpPrice = _house.LevelPrice;
            }

            houseLevelUpButton.onClick.RemoveAllListeners();
            houseLevelUpButton.onClick.AddListener(_house.LevelUp);

            DestroyPrice = (int)(BuildManager.Instance.HousePrice * 0.8f * _house.Level);
            houseDestroyButton.onClick.RemoveAllListeners();
            houseDestroyButton.onClick.AddListener(_house.DestroyHouse);
            
            levelLocalizedString.RefreshString();
            levelUpLocalizedString.RefreshString();
            destroyLocalizedString.RefreshString();
            
            _house.OnResidentUpdate += UpdateHouseInhabitants;
            _house.OnLevelUpdate += UpdateHouseLevel;
        }
        
        private void UpdateHouseInhabitants()
        {
            houseInhabitantsTMP.text = _house.Inhabitants.ToString();
            houseCostsPerSecondTMP.text = _house.CostsPerSecond.ToString();
            houseMultiplierTMP.text = "X" + _house.Multiplier.ToString(CultureInfo.InvariantCulture).Replace(",", ".");
            houseGoldPerSecondTMP.text = ((int)(_house.CurrentGoldPerSecond * _house.Multiplier)).ToString();
        }
        
        private void UpdateHouseLevel()
        {
            Level = _house.Level;
            houseInhabitantsTMP.text = _house.Inhabitants.ToString();
            houseMaxInhabitantsTMP.text = _house.MaxInhabitants.ToString();
            houseCostsPerSecondTMP.text = _house.CostsPerSecond.ToString();
            houseMultiplierTMP.text = "X" + _house.Multiplier.ToString(CultureInfo.InvariantCulture).Replace(",", ".");
            houseGoldPerSecondTMP.text = ((int)(_house.CurrentGoldPerSecond * _house.Multiplier)).ToString();
            
            if (_house.Level > 2)
            {
                houseLevelUpButtonTMP.text = maxLevelLocalizedString.GetLocalizedString();
                houseLevelUpButton.interactable = false;
            }
            else
            {
                houseLevelUpButton.interactable = true;
                LevelUpPrice = _house.LevelPrice;
            }
        
            DestroyPrice = (int)(BuildManager.Instance.HousePrice * 0.8f * _house.Level);
            
            levelLocalizedString.RefreshString();
            levelUpLocalizedString.RefreshString();
            destroyLocalizedString.RefreshString();
        }
        
        private void UpdateLevelString(string s)
        {
            houseLevelTMP.text = s;
        }

        private void UpdateLevelUpButtonString(string s)
        {
            houseLevelUpButtonTMP.text = s;
        }

        private void UpdateDestroyButtonTMP(string s)
        {
            houseDestroyButtonTMP.text = s;
        }
        
        #endregion
    }
}