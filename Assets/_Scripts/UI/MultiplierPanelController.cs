
using System;
using System.Globalization;
using Buildings;
using Commons;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using Utilities;

namespace UI
{ 
	public class MultiplierPanelController : UIPanel
	{
		#region Inspector Variables
		
		[Header("TMP texts")] 
		[SerializeField] private TMP_Text multiplierTitleTMP;
		[SerializeField] private TMP_Text multiplierLevelTMP;
		[SerializeField] private TMP_Text multiplierCostsPerSecondTMP;
		[SerializeField] private TMP_Text multiplierMultiplierTMP;
		[SerializeField] private TMP_Text multiplierEffectAreaTMP;
		[SerializeField] private TMP_Text multiplierAffectedHousesTMP;
		[SerializeField] private TMP_Text multiplierDescriptionTMP;

		[Header("Buttons")]
		[SerializeField] private TMP_Text multiplierLevelUpButtonTMP;
		[SerializeField] private Button multiplierLevelUpButton;
		[SerializeField] private TMP_Text multiplierDestroyButtonTMP;
		[SerializeField] private Button multiplierDestroyButton;

		[Header("Localized Strings")] 
		[SerializeField] private LocalizedString levelLocalizedString;
		[SerializeField] private LocalizedString maxLevelLocalizedString;
		[SerializeField] private LocalizedString levelUpLocalizedString;
		[SerializeField] private LocalizedString destroyLocalizedString;
		
		[Header("Localized Title")]
		[SerializeField] private LocalizedString multiplierPlaygroundTitleLocalizedString;
		[SerializeField] private LocalizedString multiplierPoliceTitleLocalizedString;
		[SerializeField] private LocalizedString multiplierHospitalTitleLocalizedString;
		
		[Header("Localized Description")]
		[SerializeField] private LocalizedString playgroundDescription;
		[SerializeField] private LocalizedString policeDescription;
		[SerializeField] private LocalizedString hospitalDescription;
		
		#endregion
	
		#region Public Variables
		
		public int DestroyPrice { get; private set; }
		public int LevelUpPrice { get; private set; }
		public int Level { get; private set; }
		
		#endregion

		#region Private Variables

		private MultiplierBuildingFunctionality _multiplierBuilding;
		
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

			OwnAnimateUI.OnShowAnimationPlay += ConfigurePanel;
		}
		
		private void OnDestroy()
		{
			levelLocalizedString.StringChanged -= UpdateLevelString;
			levelUpLocalizedString.StringChanged -= UpdateLevelUpButtonString;
			destroyLocalizedString.StringChanged -= UpdateDestroyButtonTMP;
            
			OwnAnimateUI.OnShowAnimationPlay -= ConfigurePanel;
		}
		
		#endregion

		#region Public Methods
		
		public override void ConfigurePanel(Building multiplier)
		{
			// If hit same multiplier when is opened just ignore
			if (OwnAnimateUI.IsOpen && _multiplierBuilding.IsNotNull() && _multiplierBuilding.gameObject.name.Equals(multiplier.gameObject.name)) return;
			if(_multiplierBuilding) OwnAnimateUI.OnHideAnimationPlay -= _multiplierBuilding.HideArea;
			OwnAnimateUI.OnHideAnimationPlay += ((MultiplierBuildingFunctionality)multiplier).HideArea;
			_multiplierBuilding = (MultiplierBuildingFunctionality) multiplier;
			
			UIManagerInGame.Instance.ChangeHUDPanel(HUDPanels.multiplierPanel);
		}
		
		#endregion

		#region Private Methods
		
		private void ConfigurePanel()
        {
            _multiplierBuilding.ShowArea();
	        SetTitle();
            Level = _multiplierBuilding.Level;
            multiplierCostsPerSecondTMP.text = _multiplierBuilding.CostsPerSecond.ToString();
            multiplierMultiplierTMP.text = "X" + _multiplierBuilding.Multiplier.ToString(CultureInfo.InvariantCulture).Replace(",", ".");
            multiplierEffectAreaTMP.text = _multiplierBuilding.EffectArea.ToString();
            multiplierAffectedHousesTMP.text = _multiplierBuilding.AffectedHouses.ToString();
            SetDescription();
            
            if (_multiplierBuilding.Level > 2)
            {
                multiplierLevelUpButtonTMP.text = maxLevelLocalizedString.GetLocalizedString();
                multiplierLevelUpButton.interactable = false;
            }
            else
            {
                multiplierLevelUpButton.interactable = true;
                LevelUpPrice = _multiplierBuilding.LevelPrice;
            }

            multiplierLevelUpButton.onClick.RemoveAllListeners();
            multiplierLevelUpButton.onClick.AddListener(LevelUpMultiplier);

            DestroyPrice = (int)(_multiplierBuilding.BuyPrice * 0.8f * _multiplierBuilding.Level);
            multiplierDestroyButton.onClick.RemoveAllListeners();
            multiplierDestroyButton.onClick.AddListener(DemolishHouse);
            
            levelLocalizedString.RefreshString();
            levelUpLocalizedString.RefreshString();
            destroyLocalizedString.RefreshString();
            
            _multiplierBuilding.OnLevelUpdate += UpdateMultiplierBuildingLevel;
        }

		private void SetTitle()
		{
			switch (_multiplierBuilding.BuildingType)
			{
				case BuildingType.playground:
					multiplierTitleTMP.text = multiplierPlaygroundTitleLocalizedString.GetLocalizedString();
					break;
				case BuildingType.hospital:
					multiplierTitleTMP.text = multiplierHospitalTitleLocalizedString.GetLocalizedString();
					break;
				case BuildingType.police:
					multiplierTitleTMP.text = multiplierPoliceTitleLocalizedString.GetLocalizedString();
					break;
				case BuildingType.road:
				case BuildingType.house:
				case BuildingType.none:
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void SetDescription()
		{
			switch (_multiplierBuilding.BuildingType)
			{
				case BuildingType.playground:
					multiplierDescriptionTMP.text = playgroundDescription.GetLocalizedString();
					break;
				case BuildingType.hospital:
					multiplierDescriptionTMP.text = hospitalDescription.GetLocalizedString();
					break;
				case BuildingType.police:
					multiplierDescriptionTMP.text = policeDescription.GetLocalizedString();
					break;
				case BuildingType.road:
				case BuildingType.house:
				case BuildingType.none:
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		
        private void DemolishHouse()
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.buttonClick);
            _multiplierBuilding.Demolish();
        }

        private void LevelUpMultiplier()
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.buttonClick);
			_multiplierBuilding.LevelUp();
        }
        
        private void UpdateMultiplierBuildingLevel()
        {
            Level = _multiplierBuilding.Level;
            multiplierCostsPerSecondTMP.text = _multiplierBuilding.CostsPerSecond.ToString();
            multiplierMultiplierTMP.text = "X" + _multiplierBuilding.Multiplier.ToString(CultureInfo.InvariantCulture).Replace(",", ".");
            multiplierEffectAreaTMP.text = _multiplierBuilding.EffectArea.ToString();
            multiplierAffectedHousesTMP.text = _multiplierBuilding.AffectedHouses.ToString();
            
            if (_multiplierBuilding.Level > 2)
            {
                multiplierLevelUpButtonTMP.text = maxLevelLocalizedString.GetLocalizedString();
                multiplierLevelUpButton.interactable = false;
            }
            else
            {
                multiplierLevelUpButton.interactable = true;
                LevelUpPrice = _multiplierBuilding.LevelPrice;
            }
        
            DestroyPrice = (int)(BuildManager.Instance.HousePrice * 0.8f * _multiplierBuilding.Level);
            
            levelLocalizedString.RefreshString();
            levelUpLocalizedString.RefreshString();
            destroyLocalizedString.RefreshString();
        }
        
        private void UpdateLevelString(string s)
        {
            multiplierLevelTMP.text = s;
        }

        private void UpdateLevelUpButtonString(string s)
        {
            multiplierLevelUpButtonTMP.text = s;
        }

        private void UpdateDestroyButtonTMP(string s)
        {
            multiplierDestroyButtonTMP.text = s;
        }
		
		#endregion
	}
}