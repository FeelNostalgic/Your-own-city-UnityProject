using System.Collections.Generic;
using Proyecto.Utilities;
using UnityEngine;

namespace Proyecto.Building
{
    public class BuildingsManager : Singlenton<BuildingsManager>
    {
        #region Inspector Variables

        #endregion

        #region Public Variables

        [HideInInspector] public List<ParqueFunctionality> Parques;
        [HideInInspector] public List<HospitalFunctionality> Hospitales;
        [HideInInspector] public List<PoliciaFunctionality> Policia;

        #endregion

        #region Private Variables

        #endregion

        #region Unity Methods

        #endregion

        #region Public Methods

        public void AddCasa()
        {
            foreach (var p in Parques)
            {
                p.UpdateMultiplicadorNewVecinos();
            }

            foreach (var h in Hospitales)
            {
                h.UpdateMultiplicadorNewVecinos();
            }

            foreach (var p in Policia)
            {
                p.UpdateMultiplicadorNewVecinos();
            }
        }

        public void RemoveCasa(GameObject casa)
        {
            foreach (var p in Parques)
            {
                p.RemoveCasa(casa);
            }

            foreach (var h in Hospitales)
            {
                h.RemoveCasa(casa);
            }

            foreach (var p in Policia)
            {
                p.RemoveCasa(casa);
            }
        }

        public void RestartBuildings()
        {
            Parques = new List<ParqueFunctionality>();
            Hospitales = new List<HospitalFunctionality>();
            Policia = new List<PoliciaFunctionality>();
        }

        #endregion

        #region Private Methods

        #endregion
    }
}