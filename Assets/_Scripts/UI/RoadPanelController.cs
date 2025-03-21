using Buildings;
using Commons;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using Utilities;

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

		#region Private Variables

		private RoadFunctionality _road;

		#endregion
		
		#region Unity Methods
		
		protected override void Awake()
		{
			base.Awake();
			destroyLocalizedString.Arguments = new object[] { this };
			destroyLocalizedString.StringChanged += UpdateString;
			
			OwnAnimateUI.OnShowAnimationPlay += ConfigurePanel;
		}

		private void OnDestroy()
		{
			destroyLocalizedString.StringChanged -= UpdateString;
			OwnAnimateUI.OnShowAnimationPlay -= ConfigurePanel;
		}

		#endregion

		#region Public Methods

		public override void ConfigurePanel(Building roadFunctionality)
		{
			if (OwnAnimateUI.IsOpen && _road.IsNotNull() && _road.gameObject.name.Equals(roadFunctionality.gameObject.name)) return;
			_road = (RoadFunctionality) roadFunctionality;
			UIManagerInGame.Instance.ChangeHUDPanel(HUDPanels.roadPanel);
		}
		
		#endregion
		
		#region Private Methods

		private void ConfigurePanel()
		{
			destroyRoadButton.onClick.RemoveAllListeners();
			destroyRoadButton.onClick.AddListener(_road.Demolish);
		}
		
		private void UpdateString(string s)
		{
			destroyRoadTMP.text = s;
		}
		
		#endregion
	}
}