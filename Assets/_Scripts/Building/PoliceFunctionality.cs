using Managers;
using UnityEngine;

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

        protected override GameObject GetParentGameObject()
        {
            return gameObject;
        }

        #endregion
    }
}