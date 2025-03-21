using System;
using System.Collections.Generic;
using System.Linq;
using Buildings;
using Commons;
using Utilities;
using UnityEngine;

namespace Managers
{
    public class BuildManager : MonoBehaviourSinglenton<BuildManager>
    {
        #region Inspector Variables
        
        [SerializeField] private List<BuildingPrefab> buildingPrefabs = new List<BuildingPrefab>();

        [Tooltip("Construction height of buildings")] [SerializeField]
        private float yBuilding;

        [SerializeField] private float housePrice;
        [SerializeField] private float roadPrice;
        [SerializeField] private float hospitalPrice;
        [SerializeField] private float policePrice;

        [SerializeField] private float playgroundPrice;

        #endregion

        #region Public Variables

        public float HousePrice => housePrice;
        public float RoadPrice => roadPrice;
        public float HospitalPrice => hospitalPrice;
        public float PolicePrice => policePrice;
        public float PlaygroundPrice => playgroundPrice;
        public float YBuilding => yBuilding;

        public static BuildingType ActiveBuildingType { get; private set; }
        public static BuildingStatus Status { get; private set; }

        public bool IsFirstRoadBuild
        {
            set => _isFirstRoadBuild = value;
        }
        
        #endregion

        #region Private Variables

        private Dictionary<BuildingType, BuildingPrefab> _buildingPrefabsDic = new();
        private bool _isFirstRoadBuild;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _buildingPrefabsDic = buildingPrefabs.ToDictionary(x => x.type, x => x);
        }

        #endregion

        #region Public Methods

        public static bool ToggleBuilding(BuildingType type)
        {
            if (type == ActiveBuildingType)
            {
                Debug.Log("Building is not active");
                Status = BuildingStatus.none;
                ActiveBuildingType = BuildingType.none;
                return false;
            }

            Debug.Log($"Building is active with {type}");
            Status = BuildingStatus.building;
            ActiveBuildingType = type;
            return true;
        }

        public static bool ToggleDemolish()
        {
            Status = Status == BuildingStatus.demolishing ? BuildingStatus.none : BuildingStatus.demolishing;

            return Status == BuildingStatus.demolishing;
        }

        public void BuildBuilding(TileFunctionality tile)
        {
            switch (ActiveBuildingType)
            {
                case BuildingType.none:
                    break;
                case BuildingType.house:
                    BuildHouse(tile);
                    break;
                case BuildingType.playground:
                    BuildPlayground(tile);
                    break;
                case BuildingType.hospital:
                    BuildHospital(tile);
                    break;
                case BuildingType.police:
                    BuildPolice(tile);
                    break;
                case BuildingType.road:
                    BuildRoad(tile);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region Private Methods

        private void BuildRoad(TileFunctionality tile)
        {
            if (ResourcesManager.Instance.CurrentGold >= roadPrice)
            {
                //Check if is possible to build a road
                if (!_isFirstRoadBuild)
                {
                    //Check if currentTile is neighbour of the firstRoad
                    var (i, j) = tile.MapPosition;
                    if (i == 0 && j.Equals(MapManager.RoadJ))
                    {
                        BuildRoadAtMap(i,j);
                        _isFirstRoadBuild = true;
                    }
                    else
                    {
                        UIManagerInGame.Instance.ShowRoadNotConnectedToRoadFeedback();
                    }
                }
                else
                {
                    var neighbours = MapManager.Instance.Get4Neighbours(tile, BuildingType.road);
                    if (neighbours.Count > 0)
                    {
                        var (i, j) = tile.MapPosition;
                        BuildRoadAtMap(i,j);
                        return;
                    }

                    UIManagerInGame.Instance.ShowRoadNotConnectedToRoadFeedback();
                }
            }
            else
            {
                UIManagerInGame.Instance.ShowNotEnoughGoldFeedback();
            }
        }

        private void BuildHouse(TileFunctionality tile)
        {
            if (ResourcesManager.Instance.CurrentGold >= housePrice)
            {
                var neighbours = MapManager.Instance.Get4Neighbours(tile, BuildingType.road);
                if (neighbours.Count > 0)
                {
                    var (i, j) = tile.MapPosition;
                    var rotation = CalculateRotation(neighbours, tile.MapPosition);
                    BuildBuildingAtMapTile(i, j, BuildingType.house, rotation);
                    ResourcesManager.Instance.AddGold((int)-housePrice);
                    PointAndClickManager.DisableCurrentLineRendererSelected();
                    return;
                }

                UIManagerInGame.Instance.ShowRoadNotConnectedToBuildingFeedback();
                return;
            }

            UIManagerInGame.Instance.ShowNotEnoughGoldFeedback();
        }

        private void BuildPlayground(TileFunctionality tile)
        {
            if (ResourcesManager.Instance.CurrentGold >= playgroundPrice)
            {
                var neighbours = MapManager.Instance.Get4Neighbours(tile, BuildingType.road);
                if (neighbours.Count > 0)
                {
                    var (i, j) = tile.MapPosition;
                    var rotation = CalculateRotation(neighbours, tile.MapPosition);
                    BuildBuildingAtMapTile(i, j, BuildingType.playground, rotation);
                    ResourcesManager.Instance.AddGold((int)-playgroundPrice);
                    return;
                }

                UIManagerInGame.Instance.ShowRoadNotConnectedToBuildingFeedback();
                return;
            }

            UIManagerInGame.Instance.ShowNotEnoughGoldFeedback();
        }

        private void BuildHospital(TileFunctionality tile)
        {
            if (ResourcesManager.Instance.CurrentGold >= hospitalPrice)
            {
                var neighbours = MapManager.Instance.Get4Neighbours(tile, BuildingType.road);
                if (neighbours.Count > 0)
                {
                    var (i, j) = tile.MapPosition;
                    var rotation = CalculateRotation(neighbours, tile.MapPosition);
                    BuildBuildingAtMapTile(i, j, BuildingType.hospital, rotation);
                    ResourcesManager.Instance.AddGold((int)-hospitalPrice);
                    return;
                }

                UIManagerInGame.Instance.ShowRoadNotConnectedToBuildingFeedback();
                return;
            }

            UIManagerInGame.Instance.ShowNotEnoughGoldFeedback();
        }

        private void BuildPolice(TileFunctionality tile)
        {
            if (ResourcesManager.Instance.CurrentGold >= policePrice)
            {
                var neighbours = MapManager.Instance.Get4Neighbours(tile, BuildingType.road);
                if (neighbours.Count > 0)
                {
                    var (i, j) = tile.MapPosition;
                    var rotation = CalculateRotation(neighbours, tile.MapPosition);
                    BuildBuildingAtMapTile(i, j, BuildingType.police, rotation);
                    ResourcesManager.Instance.AddGold((int)-policePrice);
                    return;
                }

                UIManagerInGame.Instance.ShowRoadNotConnectedToBuildingFeedback();
                return;
            }

            UIManagerInGame.Instance.ShowNotEnoughGoldFeedback();
        }

        private static Quaternion CalculateRotation(List<TileFunctionality> vecinos, Vector2Int position)
        {
            var neighbour = MapManager.Instance.GetTilePositionInMap(vecinos[0].transform.position.x, vecinos[0].transform.position.z);

            //Left
            if ((neighbour.x + 1).Equals(position.x))
            {
                return Quaternion.AngleAxis(90, Vector3.up);
            }

            //Up
            if ((neighbour.y - 1).Equals(position.y))
            {
                return Quaternion.AngleAxis(180, Vector3.up);
            }

            //Right
            if ((neighbour.x - 1).Equals(position.x))
            {
                return Quaternion.AngleAxis(270, Vector3.up);
            }

            //Down
            if ((neighbour.y + 1).Equals(position.y))
            {
                return Quaternion.identity;
            }

            return Quaternion.identity;
        }

        private void BuildRoadAtMap(int i, int j)
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.buildBuilding);
            var newBuilding = Instantiate(_buildingPrefabsDic[BuildingType.road].buildingPrefab, MapManager.Instance.MapTiles[i, j].transform);
            newBuilding.transform.position = Vector3.zero;
            newBuilding.name = BuildingType.road + "[" + i + ", " + j + "]";
            MapManager.SetTileToNewBuilding(i,j, newBuilding.GetComponent<Building>(), BuildingType.road);
            MapManager.Instance.ChangeTileToRoad(i,j);
            ResourcesManager.Instance.AddGold((int)-roadPrice);
        }

        private void BuildBuildingAtMapTile(int i, int j, BuildingType type, Quaternion rotation)
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.buildBuilding);
            var newBuilding = Instantiate(_buildingPrefabsDic[type].buildingPrefab, MapManager.Instance.MapTiles[i, j].transform);
            newBuilding.transform.position = new Vector3(newBuilding.transform.position.x, yBuilding, newBuilding.transform.position.z);
            newBuilding.transform.rotation = rotation;
            newBuilding.name = type + "[" + i + ", " + j + "]";
            MapManager.SetTileToNewBuilding(i,j, newBuilding.GetComponent<Building>(), type);
        }

        #endregion
    }

    [Serializable]
    public struct BuildingPrefab
    {
        public BuildingType type;
        public GameObject buildingPrefab;
    }
}