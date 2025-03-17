using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{ 
	public class ButtonHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		public void OnPointerEnter(PointerEventData eventData)
		{
			eventData.selectedObject = null;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			eventData.selectedObject = gameObject;
		}
	}
}