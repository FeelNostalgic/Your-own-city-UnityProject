using System;
using System.Collections.Generic;
using Buildings;
using Residents;
using Utilities;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Managers
{
    public class InhabitantsManager : MonoBehaviourSingleton<InhabitantsManager>
    {
        #region Inspector Variables

        [SerializeField] private List<ResidentIA> residentPrefabs;
        [Range(1, 5)] [SerializeField] private float minSecondsBetweenResidents = 1f;
        [Range(2, 10)] [SerializeField] private float maxSecondsBetweenResidents = 3f;

        #endregion

        #region Public Variables

        public Vector3 RoadPosition
        {
            set => _roadPosition = value;
        }

        #endregion

        #region Private Variables

        private Vector3 _roadPosition; //SpawnPoint
        private static readonly Queue<HouseFunctionality> AvailableHouses = new();
        private float _timerToSpawnResident;
        
        
        #endregion

        #region Unity Methods

        private void Awake()
        {
            _timerToSpawnResident = Random.Range(minSecondsBetweenResidents, maxSecondsBetweenResidents);
        }

        private void Update()
        {
            GenerateResident();
        }

        #endregion

        #region Public Methods

        public static void AddAvailableHouse(HouseFunctionality houseFunctionality)
        {
            AvailableHouses.Enqueue(houseFunctionality);
        } 
        
        #endregion

        #region Private Methods
        
        private void GenerateResident()
        {
            if (AvailableHouses.Count == 0) return;
            if (_timerToSpawnResident < 0)
            {
                var timeToNextResident = Random.Range(minSecondsBetweenResidents, maxSecondsBetweenResidents);
                // Debug.Log($"New resident generated. Time to next: {timeToNextResident}");
                _timerToSpawnResident = timeToNextResident;
                if(GetNextAvailableHouse(out var house)) Instance.SummonResident(house);
            }

            _timerToSpawnResident -= Time.deltaTime;
        }

        private static bool GetNextAvailableHouse(out HouseFunctionality house)
        {
            house = AvailableHouses.Dequeue();
            while (house.IsNull() && AvailableHouses.Count > 0)
            {
                house = AvailableHouses.Dequeue();
            }

            return house;
        }
        
        private void SummonResident(HouseFunctionality house)
        {
            var resident = Instantiate(residentPrefabs[Random.Range(0, 3)], _roadPosition + (Vector3.up * 15f), Quaternion.identity);
            resident.BuildPath(house);
        }
        
        #endregion
    }
}