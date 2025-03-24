
using Managers;

namespace Buildings
{ 
	public class RoadFunctionality : Building
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
		
		public override void Demolish()
		{
			ResourcesManager.AddGold((int)(BuildManager.Instance.RoadPrice * 0.8));
			MapManager.Instance.DemolishRoad(transform.parent.transform.position);
			UIManagerInGame.Instance.DisableAllHUDExceptBuildPanel();
			Destroy(gameObject);
		}
		
		#endregion

		#region Private Methods
		
		#endregion


	}
}