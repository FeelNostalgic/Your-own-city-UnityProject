using System.Collections.Generic;
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
        private readonly Dictionary<BuildManager.BuildingType, Image> _buttonImages = new();
        private Image _demolishButtonImage;
        
        #endregion
        
        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();
            SetPricesInBuildPanel();
            InitializeButtonListeners();
            GetButtonImagesComponent();
            _buttonDefaultSprite = _buttonImages[BuildManager.BuildingType.road].sprite;
            _demolishButtonImage = demolishButton.GetComponent<Image>();
            _openPanelRectTransformTMP = openPanelButton.GetComponentInChildren<TMP_Text>().GetComponent<RectTransform>();
            OwnAnimateUI.OnShowAnimationPlay += RotateOpenButton;
            OwnAnimateUI.OnHideAnimationPlay += RotateOpenButton;
            OwnAnimateUI.OnHideAnimationCompleted += SetBuildingModeToNone;
        }
        
        private void OnDestroy()
        {
            OwnAnimateUI.OnShowAnimationPlay -= RotateOpenButton;
            OwnAnimateUI.OnHideAnimationPlay -= RotateOpenButton;
            OwnAnimateUI.OnHideAnimationCompleted -= SetBuildingModeToNone;
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
            
            roadButton.onClick.AddListener(()=>ToggleBuildingType(BuildManager.BuildingType.road));
            houseButton.onClick.AddListener(()=>ToggleBuildingType(BuildManager.BuildingType.house));
            playgroundButton.onClick.AddListener(()=>ToggleBuildingType(BuildManager.BuildingType.playground));
            hospitalButton.onClick.AddListener(()=>ToggleBuildingType(BuildManager.BuildingType.hospital));
            policeButton.onClick.AddListener(()=>ToggleBuildingType(BuildManager.BuildingType.police));
        }

        private void TogglePanel()
        {
            PlayClickSound();
            if (OwnAnimateUI.IsOpen)
            {
                OwnAnimateUI.Hide();
                UIManagerInGame.Instance.ChangeHUDPanel(UIManagerInGame.HUDPanels.none);
            }
            else
            {
                UIManagerInGame.Instance.ChangeHUDPanel(UIManagerInGame.HUDPanels.buildPanel);
                PointAndClickManager.DisableCurrentLineRendererSelected();
            }
        }
        
        private void ToggleDemolish()
        {
            PlayClickSound();
            if (BuildManager.Status == BuildManager.BuildingStatus.building)
            {
                _buttonImages[BuildManager.ActiveBuildingType].sprite = _buttonDefaultSprite;
                BuildManager.ToggleBuilding(BuildManager.ActiveBuildingType);
            }
            var isDemolishActive = BuildManager.ToggleDemolish();
            _demolishButtonImage.sprite = isDemolishActive ? buttonPressedSprite : _buttonDefaultSprite;
        }
        
        private void RotateOpenButton()
        {
            _openPanelRectTransformTMP.rotation = Quaternion.Euler(0, 0, OwnAnimateUI.IsOpen ? 180 : 0);
        }
        
        private void ToggleBuildingType(BuildManager.BuildingType type)
        {
            PlayClickSound();
            if (BuildManager.Status == BuildManager.BuildingStatus.demolishing) _demolishButtonImage.sprite = _buttonDefaultSprite;
            
            // Reset previous active button (if any)
            if (BuildManager.ActiveBuildingType != BuildManager.BuildingType.none)
            {
                _buttonImages[BuildManager.ActiveBuildingType].sprite = _buttonDefaultSprite;
            }
    
            // Toggle the building type and update the button visual
            var isBuildingActive = BuildManager.ToggleBuilding(type);
            _buttonImages[type].sprite = isBuildingActive ? buttonPressedSprite : _buttonDefaultSprite;
        }
        
        private void SetBuildingModeToNone()
        {
            if (BuildManager.ActiveBuildingType == BuildManager.BuildingType.none) return;
            var type = BuildManager.ActiveBuildingType;
            var isActive = BuildManager.ToggleBuilding(BuildManager.ActiveBuildingType);
            _buttonImages[type].sprite = isActive ? buttonPressedSprite : _buttonDefaultSprite;
        }
        
        private void GetButtonImagesComponent()
        {
            _buttonImages.Add(BuildManager.BuildingType.road, roadButton.GetComponent<Image>());
            _buttonImages.Add(BuildManager.BuildingType.house, houseButton.GetComponent<Image>());
            _buttonImages.Add(BuildManager.BuildingType.playground, playgroundButton.GetComponent<Image>());
            _buttonImages.Add(BuildManager.BuildingType.hospital, hospitalButton.GetComponent<Image>());
            _buttonImages.Add(BuildManager.BuildingType.police, policeButton.GetComponent<Image>());
        }

        private void PlayClickSound()
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.buttonClick);
        }

        #endregion
    }
}