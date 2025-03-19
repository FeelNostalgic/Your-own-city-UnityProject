using System.Linq;
using Managers;
using UnityEngine;

namespace Buildings
{
    public class PlaygroundFunctionality : MultiplierBuildingFunctionality
    {
        #region Unity Methods

        protected override void Start()
        {
            _buildingPrice = BuildManager.Instance.PlaygroundPrice;
            base.Start();
        }

        #endregion

        #region Protected Methods

        protected override void RegisterBuilding()
        {
            BuildingsManager.Instance.Playgrounds.Add(this);
        }

        protected override void UnregisterBuilding()
        {
            BuildingsManager.Instance.Playgrounds.Remove(this);
        }

        protected override GameObject GetParentGameObject()
        {
            return gameObject;
        }

        public override void UpdateMultiplierNewNeighbours()
        {
            foreach (var v in _neighbourTiles.Where(v => v != null && v.GetComponent<BuildType>().type == BuildManager.BuildingType.house && !_affectedHouses.Contains(v)))
            {
                _affectedHouses.Add(v);
                v.GetComponentInChildren<HouseFunctionality>().UpgradeMultiplier(_currentMultiplier - 1);
            }
        }

        #endregion
    }
}