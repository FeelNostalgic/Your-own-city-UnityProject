using System.Collections.Generic;
using Buildings;
using Utilities;
using UnityEngine;

namespace Managers
{
    public class BuildManager : MonoBehaviourSinglenton<BuildManager>
    {
        #region Inspector Variables

        [Tooltip("Order: house, Playground, Hospital, Police")] [SerializeField]
        private GameObject[] buildingsPrefab;

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

        public BuildingType ActiveBuildingType { get; private set; }

        public bool IsFirstRoadBuild
        {
            set => _isFirstRoadBuild = value;
        }

        public enum BuildingType
        {
            none,
            house,
            playground,
            hospital,
            police,
            road
        }

        #endregion

        #region Private Variables

        private GameObject _currentTile;
        private bool _isFirstRoadBuild;

        #endregion

        #region Unity Methods

        // EMPTY

        #endregion

        #region Public Methods

        public void SetCurrentTile(GameObject tile)
        {
            _currentTile = tile;
        }

        public bool ToggleBuilding(BuildingType type)
        {
            if (type == ActiveBuildingType)
            {
                ActiveBuildingType = BuildingType.none;
                return false;
            }
            ActiveBuildingType = type;
            return true;
        }
        public void BuildRoad()
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.buttonClick);
            if (ResourcesManager.Instance.CurrentGold >= roadPrice)
            {
                //Check if is possible to build a road
                if (!_isFirstRoadBuild)
                {
                    //Check if currentTile is neighbour of the firstRoad
                    var tilePosition = MapManager.Instance.TilePosition(_currentTile.transform.position.x, _currentTile.transform.position.z);
                    if (tilePosition.x == 0 && Mathf.Approximately(tilePosition.y, MapManager.Instance.RoadZ))
                    {
                        var newTile = MapManager.Instance.BuildRoadAtMap((int)tilePosition.x, (int)tilePosition.y);
                        newTile.GetComponent<BuildType>().type = BuildingType.road;

                        MapManager.Instance.NavMeshSurface.BuildNavMesh();

                        UIManagerInGame.Instance.ShowBuildPanel(false);
                        ResourcesManager.Instance.AddGold((int)-roadPrice);
                        _isFirstRoadBuild = true;
                    }
                    else
                    {
                        UIManagerInGame.Instance.ShowRoadNotConnectedToRoadFeedback();
                    }
                }
                else
                {
                    var neighbours = MapManager.Instance.Get4Neighbours(_currentTile, BuildingType.road);
                    if (neighbours.Count > 0)
                    {
                        var tilePosition = MapManager.Instance.TilePosition(_currentTile.transform.position.x,
                            _currentTile.transform.position.z);
                        var newTile = MapManager.Instance.BuildRoadAtMap((int)tilePosition.x, (int)tilePosition.y);
                        newTile.GetComponent<BuildType>().type = BuildingType.road;

                        MapManager.Instance.NavMeshSurface
                            .UpdateNavMesh(MapManager.Instance.NavMeshSurface.navMeshData);

                        UIManagerInGame.Instance.ShowBuildPanel(false);
                        ResourcesManager.Instance.AddGold((int)-roadPrice);
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

        public void BuildHouse()
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.buttonClick);
            if (ResourcesManager.Instance.CurrentGold >= housePrice)
            {
                var neighbours = MapManager.Instance.Get4Neighbours(_currentTile, BuildingType.road);
                if (neighbours.Count > 0)
                {
                    var tilePosition = MapManager.Instance.TilePosition(_currentTile.transform.position.x, _currentTile.transform.position.z);
                    var rotation = CalculateRotation(neighbours, tilePosition);
                    BuildBuilding((int)tilePosition.x, (int)tilePosition.y, BuildingType.house, rotation);
                    _currentTile.GetComponent<BuildType>().type = BuildingType.house;
                    ResourcesManager.Instance.AddGold((int)-housePrice);
                    UIManagerInGame.Instance.ShowBuildPanel(false);
                    PointAndClickManager.Instance.DisableCurrentLineRendererSelected();
                    return;
                }

                UIManagerInGame.Instance.ShowRoadNotConnectedToBuildingFeedback();
            }

            UIManagerInGame.Instance.ShowNotEnoughGoldFeedback();
        }

        public void BuildPlayground()
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.buttonClick);
            if (ResourcesManager.Instance.CurrentGold >= playgroundPrice)
            {
                var neighbours = MapManager.Instance.Get4Neighbours(_currentTile, BuildingType.road);
                if (neighbours.Count > 0)
                {
                    var tilePosition = MapManager.Instance.TilePosition(_currentTile.transform.position.x,
                        _currentTile.transform.position.z);
                    var rotation = CalculateRotation(neighbours, tilePosition);
                    BuildBuilding((int)tilePosition.x, (int)tilePosition.y, BuildingType.playground, rotation);
                    _currentTile.GetComponent<BuildType>().type = BuildingType.playground;
                    ResourcesManager.Instance.AddGold((int)-playgroundPrice);
                    UIManagerInGame.Instance.ShowBuildPanel(false);
                    return;
                }

                UIManagerInGame.Instance.ShowRoadNotConnectedToBuildingFeedback();
            }

            UIManagerInGame.Instance.ShowNotEnoughGoldFeedback();
        }

        public void BuildHospital()
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.buttonClick);
            if (ResourcesManager.Instance.CurrentGold >= hospitalPrice)
            {
                var neighbours = MapManager.Instance.Get4Neighbours(_currentTile, BuildingType.road);
                if (neighbours.Count > 0)
                {
                    var tilePosition = MapManager.Instance.TilePosition(_currentTile.transform.position.x,
                        _currentTile.transform.position.z);
                    var rotation = CalculateRotation(neighbours, tilePosition);
                    BuildBuilding((int)tilePosition.x, (int)tilePosition.y, BuildingType.hospital, rotation);
                    _currentTile.GetComponent<BuildType>().type = BuildingType.hospital;
                    ResourcesManager.Instance.AddGold((int)-hospitalPrice);
                    UIManagerInGame.Instance.ShowBuildPanel(false);
                    return;
                }

                UIManagerInGame.Instance.ShowRoadNotConnectedToBuildingFeedback();
            }

            UIManagerInGame.Instance.ShowNotEnoughGoldFeedback();
        }

        public void BuildPolice()
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.buttonClick);
            if (ResourcesManager.Instance.CurrentGold >= policePrice)
            {
                var neighbours = MapManager.Instance.Get4Neighbours(_currentTile, BuildingType.road);
                if (neighbours.Count > 0)
                {
                    var tilePosition = MapManager.Instance.TilePosition(_currentTile.transform.position.x,
                        _currentTile.transform.position.z);
                    var rotation = CalculateRotation(neighbours, tilePosition);
                    BuildBuilding((int)tilePosition.x, (int)tilePosition.y, BuildingType.police, rotation);
                    _currentTile.GetComponent<BuildType>().type = BuildingType.police;
                    ResourcesManager.Instance.AddGold((int)-policePrice);
                    UIManagerInGame.Instance.ShowBuildPanel(false);
                    return;
                }

                UIManagerInGame.Instance.ShowRoadNotConnectedToBuildingFeedback();
            }

            UIManagerInGame.Instance.ShowNotEnoughGoldFeedback();
        }

        #endregion

        #region Private Methods

        private static Quaternion CalculateRotation(List<GameObject> vecinos, Vector2 position)
        {
            var neighbour =
                MapManager.Instance.TilePosition(vecinos[0].transform.position.x, vecinos[0].transform.position.z);

            //Left
            if (Mathf.Approximately(neighbour.x + 1, position.x))
            {
                return Quaternion.AngleAxis(90, Vector3.up);
            }

            //Up
            if (Mathf.Approximately(neighbour.y - 1, position.y))
            {
                return Quaternion.AngleAxis(180, Vector3.up);
            }

            //Right
            if (Mathf.Approximately(neighbour.x - 1, position.x))
            {
                return Quaternion.AngleAxis(270, Vector3.up);
            }

            //Down
            if (Mathf.Approximately(neighbour.y + 1, position.y))
            {
                return Quaternion.identity;
            }

            return Quaternion.identity;
        }

        private void BuildBuilding(int i, int j, BuildingType type, Quaternion rotation)
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.buildBuilding);
            var newBuilding = Instantiate(buildingsPrefab[(int)type - 1], MapManager.Instance.MapTiles[i, j].transform);
            newBuilding.transform.position = new Vector3(newBuilding.transform.position.x, yBuilding,
                newBuilding.transform.position.z);
            newBuilding.transform.rotation = rotation;
            newBuilding.name = type + "[" + i + ", " + j + "]";
        }
        
        #endregion
    }
}