using System.Collections.Generic;
using Residents;
using Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public class ResidentsManager : MonoBehaviourSinglenton<ResidentsManager>
    {
        #region Inspector Variables

        [SerializeField] private List<GameObject> Prefabs;

        #endregion

        #region Public Variables

        public Vector3 RoadPosition
        {
            set => _roadPosition = value;
        }

        #endregion

        #region Private Variables

        private Vector3 _roadPosition; //SpawnPoint

        #endregion

        #region Unity Methods (EMPTY)

        #endregion

        #region Public Methods

        public void SummonHabitante(GameObject casa)
        {
            GameObject habitante = Instantiate(Prefabs[Random.Range(0, 3)], _roadPosition + (Vector3.up * 15f),
                Quaternion.identity);
            habitante.GetComponent<HabitanteIA>().BuildPath(casa);
        }

        #endregion

        #region Private Methods (EMPTY)

        #endregion
    }
}