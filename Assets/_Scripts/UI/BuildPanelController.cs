using System.Collections.Generic;
using Buildings;
using Commons;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class BuildPanelController : UIPanel
    {
        #region Inspector Variables

        [SerializeField] private Sprite buttonPressedSprite;
        
        [Header("Quick Menu Buttons")]
        [SerializeField] private Button openPanelButton;
        [SerializeField] private Button demolishButton;
        
        [Header("Buildings buttons")]
        [SerializeField] private Button roadButton;
        [SerializeField] private Button houseButton;
        [SerializeField] private Button playgroundButton;
        [SerializeField] private Button hospitalButton;
        [SerializeField] private Button policeButton;
        
        [Header("TMP components")]
        [SerializeField] private TMP_Text roadPriceTMP;
        [SerializeField] private TMP_Text housePriceTMP;
        [SerializeField] private TMP_Text playgroundPriceTMP;
        [SerializeField] private TMP_Text hospitalPriceTMP;
        [SerializeField] private TMP_Text policePriceTMP;

        #endregion

        #region Public Variables
        //
        #endregion
        
        #region Private Variables
        
        private RectTransform _openPanelRectTransformTMP;
        private Sprite _buttonDefaultSprite;
        private readonly Dictionary<BuildingType, Image> _buttonImages = new();
        private Image _demolishButtonImage;
        
        #endregion
        
        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();
            SetPricesInBuildPanel();
            InitializeButtonListeners();
            GetButtonImagesComponent();
            _buttonDefaultSprite = _buttonImages[BuildingType.road].sprite;
            _demolishButtonImage = demolishButton.GetComponent<Image>();
            _openPanelRectTransformTMP = openPanelButton.GetComponentInChildren<TMP_Text>().GetComponent<RectTransform>();
            OwnAnimateUI.OnShowAnimationPlay += RotateOpenButtonToClose;
            OwnAnimateUI.OnHideAnimationCompleted += RotateOpenButtonToOpen;
            OwnAnimateUI.OnHideAnimationCompleted += SetBuildingModeToNone;
        }
        
        private void OnDestroy()
        {
            OwnAnimateUI.OnShowAnimationPlay -= RotateOpenButtonToClose;
            OwnAnimateUI.OnHideAnimationCompleted -= RotateOpenButtonToOpen;
            OwnAnimateUI.OnHideAnimationCompleted -= SetBuildingModeToNone;
        }

        #endregion

        #region Public Methods

        public override void ConfigurePanel(Building buildingData)
        {
            //
        }

        #endregion
        
        #region Private Methods
        
        private void SetPricesInBuildPanel()
        {
            roadPriceTMP.text = $"{(int)BuildManager.Instance.RoadPrice:N0}";
            housePriceTMP.text = $"{(int)BuildManager.Instance.HousePrice:N0}";
            playgroundPriceTMP.text = $"{(int)BuildManager.Instance.PlaygroundPrice:N0}";
            housePriceTMP.text = $"{(int)BuildManager.Instance.HospitalPrice:N0}";
            policePriceTMP.text = $"{(int)BuildManager.Instance.PolicePrice:N0}";
        }

        private void InitializeButtonListeners()
        {
            openPanelButton.onClick.AddListener(TogglePanel);
            demolishButton.onClick.AddListener(ToggleDemolish);
            
            roadButton.onClick.AddListener(()=>ToggleBuildingType(BuildingType.road));
            houseButton.onClick.AddListener(()=>ToggleBuildingType(BuildingType.house));
            playgroundButton.onClick.AddListener(()=>ToggleBuildingType(BuildingType.playground));
            hospitalButton.onClick.AddListener(()=>ToggleBuildingType(BuildingType.hospital));
            policeButton.onClick.AddListener(()=>ToggleBuildingType(BuildingType.police));
        }

        private void TogglePanel()
        {
            PlayClickSound();
            if (OwnAnimateUI.IsOpen)
            {
                OwnAnimateUI.Hide();
                UIManagerInGame.Instance.ChangeHUDPanel(HUDPanels.none);
            }
            else
            {
                UIManagerInGame.Instance.ChangeHUDPanel(HUDPanels.buildPanel);
                PointAndClickManager.DisableCurrentLineRendererSelected();
            }
        }
        
        private void ToggleDemolish()
        {
            PlayClickSound();
            if (BuildManager.Status == BuildingStatus.building)
            {
                _buttonImages[BuildManager.ActiveBuildingType].sprite = _buttonDefaultSprite;
                BuildManager.ToggleBuilding(BuildManager.ActiveBuildingType);
            }
            var isDemolishActive = BuildManager.ToggleDemolish();
            _demolishButtonImage.sprite = isDemolishActive ? buttonPressedSprite : _buttonDefaultSprite;
        }
        
        private void RotateOpenButtonToOpen()
        {
            _openPanelRectTransformTMP.rotation = Quaternion.Euler(0, 0, 0);
        }

        private void RotateOpenButtonToClose()
        {
            _openPanelRectTransformTMP.rotation = Quaternion.Euler(0, 0, 180);
        }
        
        private void ToggleBuildingType(BuildingType type)
        {
            PlayClickSound();
            if (BuildManager.Status == BuildingStatus.demolishing) _demolishButtonImage.sprite = _buttonDefaultSprite;
            
            // Reset previous active button (if any)
            if (BuildManager.ActiveBuildingType != BuildingType.none)
            {
                _buttonImages[BuildManager.ActiveBuildingType].sprite = _buttonDefaultSprite;
            }
    
            // Toggle the building type and update the button visual
            var isBuildingActive = BuildManager.ToggleBuilding(type);
            _buttonImages[type].sprite = isBuildingActive ? buttonPressedSprite : _buttonDefaultSprite;
        }
        
        private void SetBuildingModeToNone()
        {
            if (BuildManager.ActiveBuildingType == BuildingType.none) return;
            var type = BuildManager.ActiveBuildingType;
            var isActive = BuildManager.ToggleBuilding(BuildManager.ActiveBuildingType);
            _buttonImages[type].sprite = isActive ? buttonPressedSprite : _buttonDefaultSprite;
        }
        
        private void GetButtonImagesComponent()
        {
            _buttonImages.Add(BuildingType.road, roadButton.GetComponent<Image>());
            _buttonImages.Add(BuildingType.house, houseButton.GetComponent<Image>());
            _buttonImages.Add(BuildingType.playground, playgroundButton.GetComponent<Image>());
            _buttonImages.Add(BuildingType.hospital, hospitalButton.GetComponent<Image>());
            _buttonImages.Add(BuildingType.police, policeButton.GetComponent<Image>());
        }

        private void PlayClickSound()
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.buttonClick);
        }

        #endregion
    }
}