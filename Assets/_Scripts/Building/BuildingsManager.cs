using System.Collections.Generic;
using Utilities;

namespace Buildings
{
    public class BuildingsManager : Singlenton<BuildingsManager>
    {
        #region Inspector Variables

        #endregion

        #region Public Variables

        public List<PlaygroundFunctionality> Playgrounds { get; private set; } = new();
        public List<HospitalFunctionality> Hospitals { get; private set; } = new();
        public List<PoliceFunctionality> Police { get; private set; } = new();

        #endregion

        #region Private Variables

        #endregion

        #region Unity Methods
        
        #endregion

        #region Public Methods

        public void AddHouse()
        {
            foreach (var p in Playgrounds)
            {
                p.UpdateMultiplierNewNeighbours();
            }

            foreach (var h in Hospitals)
            {
                h.UpdateMultiplierNewNeighbours();
            }

            foreach (var p in Police)
            {
                p.UpdateMultiplierNewNeighbours();
            }
        }

        public void RemoveCasa(TileFunctionality casa)
        {
            foreach (var p in Playgrounds)
            {
                p.RemoveHouseFromAffectedHouses(casa);
            }

            foreach (var h in Hospitals)
            {
                h.RemoveHouseFromAffectedHouses(casa);
            }

            foreach (var p in Police)
            {
                p.RemoveHouseFromAffectedHouses(casa);
            }
        }

        public void RestartBuildings()
        {
            Playgrounds = new List<PlaygroundFunctionality>();
            Hospitals = new List<HospitalFunctionality>();
            Police = new List<PoliceFunctionality>();
        }

        #endregion

        #region Private Methods

        #endregion
    }
}