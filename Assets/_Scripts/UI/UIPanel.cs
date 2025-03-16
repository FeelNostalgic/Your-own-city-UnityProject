
using UnityEngine;

namespace UI
{ 
	public abstract class UIPanel : MonoBehaviour
	{
		public float TimeBetweenAnimations => AnimateUI.Duration;
		
		protected AnimateUI AnimateUI;
		
		protected virtual void Awake()
		{
			AnimateUI = GetComponent<AnimateUI>();
		}
		
		public void ShowPanel()
		{
			AnimateUI.Show();
		}

		public void HidePanel()
		{
			AnimateUI.Hide();
		}
	}
}