
using UnityEngine;

namespace UI
{ 
	public abstract class UIPanel : MonoBehaviour
	{
		public float TimeBetweenAnimations => OwnAnimateUI.Duration;
		
		protected AnimateUI OwnAnimateUI;
		
		protected virtual void Awake()
		{
			OwnAnimateUI = GetComponent<AnimateUI>();
		}
		
		public void ShowPanel()
		{
			OwnAnimateUI.Show();
		}

		public void HidePanel()
		{
			OwnAnimateUI.Hide();
		}
	}
}