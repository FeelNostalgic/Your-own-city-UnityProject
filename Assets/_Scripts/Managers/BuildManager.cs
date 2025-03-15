using System.Collections.Generic;
using Buildings;
using Utilities;
using Unity.AI.Navigation;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public class BuildManager : MonoBehaviourSinglenton<BuildManager>
    {
        #region Inspector Variables

        [Tooltip("Orden: Casa, Parque, Hospital, Policia")] [SerializeField]
        private GameObject[] buildingsPrefab;

        [Tooltip("Altura de contruccion de los edificios")] [SerializeField]
        private float yBuilding;

        [SerializeField] private float housePrice;
        [SerializeField] private float roadPrice;
        [SerializeField] private float hospitalPrice;
        [SerializeField] private float policePrice;
        [SerializeField] private float parquePrice;

        #endregion

        #region Public Variables

        public float HousePrice => housePrice;
        public float RoadPrice => roadPrice;
        public float HospitalPrice => hospitalPrice;
        public float PolicePrice => policePrice;
        public float ParquePrice => parquePrice;
        public float YBuilding => yBuilding;

        public bool IsFirstRoadBuild
        {
            set => _isFirstRoadBuild = value;
        }

        public enum _building
        {
            none,
            casa,
            parque,
            hospital,
            policia,
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

        public void BuildCarretera()
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.buttonClick);
            if (ResourcesManager.Instance.CurrentGold >= roadPrice)
            {
                //Check si es posible contruir carreter
                if (!_isFirstRoadBuild)
                {
                    //Comprobar si currentTile es vecino de la firstRoad
                    var tilePosition = MapManager.Instance.TilePosition(_currentTile.transform.position.x,
                        _currentTile.transform.position.z);
                    if (tilePosition.x == 0 && tilePosition.y == MapManager.Instance.RoadZ)
                    {
                        var newtile = MapManager.Instance.BuildRoadAtMap((int)tilePosition.x, (int)tilePosition.y);
                        newtile.GetComponent<BuildType>().Type = _building.road;

                        MapManager.Instance.RoadParent.GetComponent<NavMeshSurface>().BuildNavMesh();

                        UIManagerInGame.Instance.ShowBuildPanel(false);
                        ResourcesManager.Instance.AddGold((int)-roadPrice);
                        _isFirstRoadBuild = true;
                    }
                    else
                    {
                        UIManagerInGame.Instance.UpdateInfoGeneral(
                            "¡La carretera tiene que estar conectada a otra carretera!");
                    }
                }
                else
                {
                    var vecinos = MapManager.Instance.GetVecinos4(_currentTile, _building.road);
                    if (vecinos.Count > 0)
                    {
                        var tilePosition = MapManager.Instance.TilePosition(_currentTile.transform.position.x,
                            _currentTile.transform.position.z);
                        var newtile = MapManager.Instance.BuildRoadAtMap((int)tilePosition.x, (int)tilePosition.y);
                        newtile.GetComponent<BuildType>().Type = _building.road;

                        MapManager.Instance.RoadParent.GetComponent<NavMeshSurface>()
                            .UpdateNavMesh(MapManager.Instance.RoadParent.GetComponent<NavMeshSurface>().navMeshData);

                        UIManagerInGame.Instance.ShowBuildPanel(false);
                        ResourcesManager.Instance.AddGold((int)-roadPrice);
                        return;
                    }

                    UIManagerInGame.Instance.UpdateInfoGeneral("¡La carretera tiene que estar conectada a otra carretera!");
                }
            }
            else
            {
                UIManagerInGame.Instance.UpdateInfoGeneral("¡Gold insuficiente!");
            }
        }

        public void BuildCasa()
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.buttonClick);
            if (ResourcesManager.Instance.CurrentGold >= housePrice)
            {
                var vecinos = MapManager.Instance.GetVecinos4(_currentTile, _building.road);
                if (vecinos.Count > 0)
                {
                    var tilePosition = MapManager.Instance.TilePosition(_currentTile.transform.position.x,
                        _currentTile.transform.position.z);
                    Quaternion rotation = CalculateRotation(vecinos, tilePosition);
                    BuildBuilding((int)tilePosition.x, (int)tilePosition.y, _building.casa, rotation);
                    _currentTile.GetComponent<BuildType>().Type = _building.casa;
                    ResourcesManager.Instance.AddGold((int)-housePrice);
                    UIManagerInGame.Instance.ShowBuildPanel(false);
                    return;
                }

                UIManagerInGame.Instance.UpdateInfoGeneral("¡El edificio debe estar conectado a una carretera!");
            }

            UIManagerInGame.Instance.UpdateInfoGeneral("¡Gold insuficiente!");
        }

        public void BuildParque()
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.buttonClick);
            if (ResourcesManager.Instance.CurrentGold >= parquePrice)
            {
                var vecinos = MapManager.Instance.GetVecinos4(_currentTile, _building.road);
                if (vecinos.Count > 0)
                {
                    var tilePosition = MapManager.Instance.TilePosition(_currentTile.transform.position.x,
                        _currentTile.transform.position.z);
                    Quaternion rotation = CalculateRotation(vecinos, tilePosition);
                    BuildBuilding((int)tilePosition.x, (int)tilePosition.y, _building.parque, rotation);
                    _currentTile.GetComponent<BuildType>().Type = _building.parque;
                    ResourcesManager.Instance.AddGold((int)-parquePrice);
                    UIManagerInGame.Instance.ShowBuildPanel(false);
                    return;
                }

                UIManagerInGame.Instance.UpdateInfoGeneral("¡El edificio debe estar conectado a una carretera!");
            }

            UIManagerInGame.Instance.UpdateInfoGeneral("¡Gold insuficiente!");
        }

        public void BuildHospital()
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.buttonClick);
            if (ResourcesManager.Instance.CurrentGold >= hospitalPrice)
            {
                var vecinos = MapManager.Instance.GetVecinos4(_currentTile, _building.road);
                if (vecinos.Count > 0)
                {
                    var tilePosition = MapManager.Instance.TilePosition(_currentTile.transform.position.x,
                        _currentTile.transform.position.z);
                    var rotation = CalculateRotation(vecinos, tilePosition);
                    BuildBuilding((int)tilePosition.x, (int)tilePosition.y, _building.hospital, rotation);
                    _currentTile.GetComponent<BuildType>().Type = _building.hospital;
                    ResourcesManager.Instance.AddGold((int)-hospitalPrice);
                    UIManagerInGame.Instance.ShowBuildPanel(false);
                    return;
                }

                UIManagerInGame.Instance.UpdateInfoGeneral("¡El edificio debe estar conectado a una carretera!");
            }

            UIManagerInGame.Instance.UpdateInfoGeneral("¡Gold insuficiente!");
        }

        public void BuildPolicia()
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.buttonClick);
            if (ResourcesManager.Instance.CurrentGold >= policePrice)
            {
                var vecinos = MapManager.Instance.GetVecinos4(_currentTile, _building.road);
                if (vecinos.Count > 0)
                {
                    var tilePosition = MapManager.Instance.TilePosition(_currentTile.transform.position.x,
                        _currentTile.transform.position.z);
                    var rotation = CalculateRotation(vecinos, tilePosition);
                    BuildBuilding((int)tilePosition.x, (int)tilePosition.y, _building.policia, rotation);
                    _currentTile.GetComponent<BuildType>().Type = _building.policia;
                    ResourcesManager.Instance.AddGold((int)-policePrice);
                    UIManagerInGame.Instance.ShowBuildPanel(false);
                    return;
                }

                UIManagerInGame.Instance.UpdateInfoGeneral("¡El edificio debe estar conectado a una carretera!");
            }

            UIManagerInGame.Instance.UpdateInfoGeneral("¡Gold insuficiente!");
        }

        #endregion

        #region Private Methods

        private Quaternion CalculateRotation(List<GameObject> vecinos, Vector2 position)
        {
            var vecino =
                MapManager.Instance.TilePosition(vecinos[0].transform.position.x, vecinos[0].transform.position.z);

            //Izquierda
            if (vecino.x + 1 == position.x)
            {
                return Quaternion.AngleAxis(90, Vector3.up);
            }
            //Up
            if (vecino.y - 1 == position.y)
            {
                return Quaternion.AngleAxis(180, Vector3.up);
            }
            //Derecha
            if (vecino.x - 1 == position.x)
            {
                return Quaternion.AngleAxis(270, Vector3.up);
            }
            //Down
            if (vecino.y + 1 == position.y)
            {
                return Quaternion.identity;
            }

            return Quaternion.identity;
        }

        private void BuildBuilding(int i, int j, _building type, Quaternion rotation)
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