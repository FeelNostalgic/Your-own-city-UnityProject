using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class BuildPanelController : UIPanel
    {
        #region Inspector Variables

        [Header("Button components")]
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

        private AnimateUI _animateUI;

        #endregion

        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();
            SetPricesInBuildPanel();
            InitializeButtonListeners();
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
            roadButton.onClick.AddListener(BuildManager.Instance.BuildRoad);
            houseButton.onClick.AddListener(BuildManager.Instance.BuildHouse);
            playgroundButton.onClick.AddListener(BuildManager.Instance.BuildPlayground);
            hospitalButton.onClick.AddListener(BuildManager.Instance.BuildHospital);
            policeButton.onClick.AddListener(BuildManager.Instance.BuildPolice);
        }

        #endregion
    }
}