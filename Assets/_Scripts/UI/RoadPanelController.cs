using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace UI
{ 
	public class RoadPanelController : UIPanel
	{
		#region Inspector Variables

		[SerializeField] private Button destroyRoadButton;
		[SerializeField] private TMP_Text destroyRoadTMP;
		[SerializeField] private LocalizedString destroyLocalizedString;
		
		#endregion
	
		#region Public Variables

		public int DestroyPrice => (int)(BuildManager.Instance.RoadPrice * 0.8);
		
		#endregion
		
		#region Unity Methods
		
		protected override void Awake()
		{
			base.Awake();
			destroyLocalizedString.Arguments = new object[] { this };
			destroyLocalizedString.StringChanged += UpdateString;
			
			destroyRoadButton.onClick.AddListener(MapManager.Instance.DemolishRoad);
		}

		private void OnDestroy()
		{
			destroyLocalizedString.StringChanged -= UpdateString;
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