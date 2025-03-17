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
        
        [Header("Button components")]
        [SerializeField] private Button openPanelButton;
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
        
        #endregion
        
        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();
            SetPricesInBuildPanel();
            InitializeButtonListeners();
            SaveButtonImagesComponent();
            _buttonDefaultSprite = _buttonImages[BuildManager.BuildingType.road].sprite;
            _openPanelRectTransformTMP = openPanelButton.GetComponentInChildren<TMP_Text>().GetComponent<RectTransform>();
            AnimateUI.OnShowAnimationCompleted += RotateOpenButton;
            AnimateUI.OnHideAnimationCompleted += RotateOpenButton;
        }
        
        private void OnDestroy()
        {
            AnimateUI.OnShowAnimationCompleted -= RotateOpenButton;
            AnimateUI.OnHideAnimationCompleted -= RotateOpenButton;
        }

        #endregion

        #region Private Methods

        private void TogglePanel()
        {
            if(AnimateUI.IsOpen) AnimateUI.Hide();
            else AnimateUI.Show();
        }

        private void RotateOpenButton()
        {
            _openPanelRectTransformTMP.rotation = Quaternion.Euler(0, 0, AnimateUI.IsOpen ? 180 : 0);
        }
        
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
            roadButton.onClick.AddListener(()=>ToggleBuildingType(BuildManager.BuildingType.road));
            houseButton.onClick.AddListener(()=>ToggleBuildingType(BuildManager.BuildingType.house));
            playgroundButton.onClick.AddListener(()=>ToggleBuildingType(BuildManager.BuildingType.playground));
            hospitalButton.onClick.AddListener(()=>ToggleBuildingType(BuildManager.BuildingType.hospital));
            policeButton.onClick.AddListener(()=>ToggleBuildingType(BuildManager.BuildingType.police));
        }

        private void ToggleBuildingType(BuildManager.BuildingType type)
        {
            // Reset previous active button (if any)
            if (BuildManager.Instance.ActiveBuildingType != BuildManager.BuildingType.none)
            {
                _buttonImages[BuildManager.Instance.ActiveBuildingType].sprite = _buttonDefaultSprite;
            }
    
            // Toggle the building type and update the button visual
            var isActive = BuildManager.Instance.ToggleBuilding(type);
            _buttonImages[type].sprite = isActive ? buttonPressedSprite : _buttonDefaultSprite;
        }
        
        private void SaveButtonImagesComponent()
        {
            _buttonImages.Add(BuildManager.BuildingType.road, roadButton.GetComponent<Image>());
            _buttonImages.Add(BuildManager.BuildingType.house, houseButton.GetComponent<Image>());
            _buttonImages.Add(BuildManager.BuildingType.playground, playgroundButton.GetComponent<Image>());
            _buttonImages.Add(BuildManager.BuildingType.hospital, hospitalButton.GetComponent<Image>());
            _buttonImages.Add(BuildManager.BuildingType.police, policeButton.GetComponent<Image>());
        }

        #endregion
    }
}