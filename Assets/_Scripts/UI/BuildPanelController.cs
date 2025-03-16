using Managers;
using TMPro;
using UnityEngine;

namespace UI
{
    public class BuildPanelController : MonoBehaviour
    {
        #region Inspector Variables

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

        private void Awake()
        {
            _animateUI = GetComponent<AnimateUI>();
            SetPricesInBuildPanel();
        }

        #endregion

        #region Public Methods

        public void ShowBuildPanel()
        {
            _animateUI.Show();
        }

        public void HideBuildPanel()
        {
            _animateUI.Hide();
        }
        
        #endregion

        #region Private Methods
        
        private void SetPricesInBuildPanel()
        {
            roadPriceTMP.text = $"{(int)BuildManager.Instance.RoadPrice:N0}";
            housePriceTMP.text = $"{(int)BuildManager.Instance.HousePrice:N0}";
            playgroundPriceTMP.text = $"{(int)BuildManager.Instance.ParquePrice:N0}";
            housePriceTMP.text = $"{(int)BuildManager.Instance.HospitalPrice:N0}";
            policePriceTMP.text = $"{(int)BuildManager.Instance.PolicePrice:N0}";
        }
        
        #endregion
    }
}