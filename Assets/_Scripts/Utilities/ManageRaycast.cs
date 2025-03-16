using UnityEngine;
using UnityEngine.EventSystems;

namespace Utilities
{
    public class ManageRaycast: MonoBehaviour, IPointerEnterHandler,  IPointerExitHandler
    {
        #region Public Methods
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            //PointAndClickManager.Instance.IsEnableRaycast = false;
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            //PointAndClickManager.Instance.IsEnableRaycast = true;
        }
        
        #endregion
    }
}