
using System;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{ 
	public class RoadPanelController : UIPanel
	{
		#region Inspector Variables

		[SerializeField] private Button destroyRoadButton;
		[SerializeField] private TMP_Text destroyRoadTMP;
		[FormerlySerializedAs("mystring")] [SerializeField] private LocalizedString myLocalizationString;
		
		#endregion
	
		#region Public Variables

		public int ReturnedValue => (int)(BuildManager.Instance.RoadPrice * 0.8);
		
		#endregion

		#region Private Variables

		private AnimateUI _animateUI;

		#endregion

		#region Unity Methods
		
		protected override void Awake()
		{
			base.Awake();
			myLocalizationString.Arguments = new object[] { this };
			myLocalizationString.StringChanged += UpdateString;
			
			destroyRoadButton.onClick.AddListener(delegate { MapManager.Instance.DestroyRoad(); });
		}

		private void OnDestroy()
		{
			myLocalizationString.StringChanged -= UpdateString;
		}

		#endregion
		
		#region Private Methods

		private void UpdateString(string s)
		{
			destroyRoadTMP.text = s;
		}
		
		#endregion
	}
}