using System.Linq;
using Commons;
using Managers;

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
        
        public override void UpdateMultiplierNewNeighbours()
        {
            foreach (var v in _neighbourTiles.Where(tileFunctionality => tileFunctionality != null && tileFunctionality.BuildingType == BuildingType.house && !_affectedHouses.Contains(tileFunctionality)))
            {
                _affectedHouses.Add(v);
                v.GetComponentInChildren<HouseFunctionality>().UpgradeMultiplier(_currentMultiplier - 1);
            }
        }

        #endregion
    }
}