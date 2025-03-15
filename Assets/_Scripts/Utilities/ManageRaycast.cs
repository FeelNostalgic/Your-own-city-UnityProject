using Managers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utilities
{
    public class ManageRaycast: MonoBehaviour, IPointerEnterHandler,  IPointerExitHandler
    {
        #region Inspector Variables

        #endregion

        #region Public Variables

        #endregion

        #region Private Variables

        #endregion

        #region Unity Methods

        #endregion

        #region Public Methods
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            PointAndClickManager.Instance.IsEnableRaycast = false;
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            PointAndClickManager.Instance.IsEnableRaycast = true;
        }
        
        #endregion

        #region Private Methods

        #endregion
        
    }
}