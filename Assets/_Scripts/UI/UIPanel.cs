
using UnityEngine;

namespace UI
{ 
	public abstract class UIPanel : MonoBehaviour
	{
		private AnimateUI _animateUI;
		
		protected virtual void Awake()
		{
			_animateUI = GetComponent<AnimateUI>();
		}
		
		public void ShowPanel()
		{
			_animateUI.Show();
		}

		public void HidePanel()
		{
			_animateUI.Hide();
		}
	}
}