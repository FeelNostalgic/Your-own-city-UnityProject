using System;
using System.Collections.Generic;
using System.Linq;
using Buildings;
using Commons;
using UnityEditor.AddressableAssets.Build.Layout;
using Utilities;
using UnityEngine;

namespace Managers
{
    public class BuildManager : MonoBehaviourSinglenton<BuildManager>
    {
        #region Inspector Variables

        [SerializeField] private List<BuildingPrefab> buildingPrefabs = new();
        [SerializeField] private List<BuildingPrefab> previewBuildings = new();

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

        private Dictionary<BuildingType, GameObject> _buildingPrefabsDic = new();
        private Dictionary<BuildingType, GameObject> _buildingPreviewsDic = new();

        private bool _isFirstRoadBuild;
        private GameObject _activeBuildingPreview;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _buildingPrefabsDic = buildingPrefabs.ToDictionary(x => x.type, x => x.buildingPrefab);
            _buildingPreviewsDic = previewBuildings.ToDictionary(x => x.type, x => x.buildingPrefab);
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


        /// <summary>
        /// Show preview in tile only if building
        /// </summary>
        /// <param name="tile">On which tile to show the preview</param>
        public void ShowBuildingPreview(TileFunctionality tile)
        {
            if (Status != BuildingStatus.building) return;
            // Check if tile has already a building
            if (tile.BuildingType == BuildingType.none)
            {
                var neighbours = MapManager.Instance.Get4Neighbours(tile, BuildingType.road);
                switch (ActiveBuildingType)
                {
                    case BuildingType.house:
                    case BuildingType.playground:
                    case BuildingType.hospital:
                    case BuildingType.police:
                        // Show preview
                        if (neighbours.Count > 0)
                        {
                            var rotation = CalculateRotation(neighbours, tile.MapPosition);
                            ShowPreview(tile.WorldPosition, rotation, ActiveBuildingType);
                            return;
                        }

                        break;
                    case BuildingType.road:
                        if (!_isFirstRoadBuild)
                        {
                            //Check if currentTile is neighbour of the firstRoad
                            var (i, j) = tile.MapPosition;
                            if (i == 0 && j.Equals(MapManager.RoadJ))
                            {
                                ShowPreview(tile.WorldPosition, ActiveBuildingType);
                                return;
                            }
                        }

                        if (neighbours.Count > 0)
                        {
                            ShowPreview(tile.WorldPosition, ActiveBuildingType);
                            return;
                        }

                        break;
                    case BuildingType.none:
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            DisablePreview();
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

            DisablePreview();
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
                        BuildRoadAtMap(tile);
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
                        BuildRoadAtMap(tile);
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
                    BuildBuildingAtMapTile(tile, BuildingType.house, rotation);
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
                    BuildBuildingAtMapTile(tile, BuildingType.playground, rotation);
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
                    BuildBuildingAtMapTile(tile, BuildingType.hospital, rotation);
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
                    BuildBuildingAtMapTile(tile, BuildingType.police, rotation);
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

        private void BuildRoadAtMap(TileFunctionality tile)
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.buildBuilding);
            var newBuilding = Instantiate(_buildingPrefabsDic[BuildingType.road], tile.Transform);
            newBuilding.transform.position = Vector3.zero;
            var (i, j) = tile.MapPosition;
            newBuilding.name = BuildingType.road + "[" + i + ", " + j + "]";
            MapManager.SetTileToNewBuilding(i, j, newBuilding.GetComponent<Building>(), BuildingType.road);
            MapManager.Instance.ChangeTileToRoad(i, j);
            ResourcesManager.Instance.AddGold((int)-roadPrice);
        }

        private void DisablePreview()
        {
            if (_activeBuildingPreview) _activeBuildingPreview.SetActive(false);
        }

        private void ShowPreview(Vector3 position, Quaternion rotation, BuildingType type)
        {
            if (_activeBuildingPreview) _activeBuildingPreview.SetActive(false);
            var preview = _buildingPreviewsDic[type];
            preview.transform.SetXZ(position.x, position.z);
            preview.transform.SetRotation(rotation);
            preview.SetActive(true);
            _activeBuildingPreview = preview;
        }

        /// <summary>
        /// Only for road
        /// </summary>
        /// <param name="position"></param>
        /// <param name="type"></param>
        private void ShowPreview(Vector3 position, BuildingType type)
        {
            if (_activeBuildingPreview) _activeBuildingPreview.SetActive(false);
            var preview = _buildingPreviewsDic[type];
            // var meshRenderer = _buildingPreviewsMeshRendererDic[type];
            //meshRenderer.material.color = isRed ? Color.red : Color.white;
            preview.transform.SetPosition(position);
            preview.SetActive(true);
            _activeBuildingPreview = preview;
        }

        private void BuildBuildingAtMapTile(TileFunctionality tile, BuildingType type, Quaternion rotation)
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.buildBuilding);
            var newBuilding = Instantiate(_buildingPrefabsDic[type], tile.Transform);
            newBuilding.transform.SetY(yBuilding);
            newBuilding.transform.rotation = rotation;
            var (i, j) = tile.MapPosition;
            newBuilding.name = type + "[" + i + ", " + j + "]";
            MapManager.SetTileToNewBuilding(i, j, newBuilding.GetComponent<Building>(), type);
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