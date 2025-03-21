using Buildings;
using UnityEngine;

namespace UI
{
    public abstract class UIPanel : MonoBehaviour
    {
        #region Protected Variables

        protected AnimateUI OwnAnimateUI;

        #endregion

        #region Public Variables

        public float TimeBetweenAnimations => OwnAnimateUI.Duration;

        #endregion

        #region Unity Methods

        protected virtual void Awake()
        {
            OwnAnimateUI = GetComponent<AnimateUI>();
        }

        #endregion

        #region Public Methods

        public void ShowPanel()
        {
            OwnAnimateUI.Show();
        }

        public void HidePanel()
        {
            OwnAnimateUI.Hide();
        }

        public abstract void ConfigurePanel(Building buildingData);

        #endregion
    }
}