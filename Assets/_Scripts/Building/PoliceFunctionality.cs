using Managers;

namespace Buildings
{
    public class PoliceFunctionality : MultiplierBuildingFunctionality
    {
        #region Unity Methods

        protected override void Start()
        {
            _buildingPrice = BuildManager.Instance.PolicePrice;
            base.Start();
        }

        #endregion

        #region Protected Methods

        protected override void RegisterBuilding()
        {
            BuildingsManager.Instance.Police.Add(this);
        }

        protected override void UnregisterBuilding()
        {
            BuildingsManager.Instance.Police.Remove(this);
        }
        
        #endregion
    }
}