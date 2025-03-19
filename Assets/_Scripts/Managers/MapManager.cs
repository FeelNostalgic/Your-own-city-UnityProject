using System;
using System.Collections.Generic;
using Controllers;
using Buildings;
using Utilities;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Managers
{
    public class MapManager : MonoBehaviourSinglenton<MapManager>
    {
        #region Inspector Variables

        [Header("MapTile")] [Tooltip("Generate a map with size: mapSize x mapSize")] [Range(8, 100)] [SerializeField]
        private int mapSize;

        [SerializeField] private Transform mapParent;

        [Header("Border")] [Tooltip("Create a exterior border to the map with this width")] [Range(8, 25)] [SerializeField]
        private int borderSize;

        [SerializeField] private Transform borderParent;

        [FormerlySerializedAs("MinNumDecoration")] [SerializeField] [Range(2, 100)]
        private int minNumDecoration;

        [FormerlySerializedAs("MaxNumDecoration")] [SerializeField] [Range(2, 100)]
        private int maxNumDecoration;

        [SerializeField] private List<GameObject> decorationGameObjectsList;

        [Header("Road")] [SerializeField] private Transform roadParent;

        [FormerlySerializedAs("x_Offset")] [Header("Offsets")] [SerializeField]
        private float xOffset;

        [FormerlySerializedAs("z_Offset")] [SerializeField]
        private float zOffset;

        [Header("Tiles")] [SerializeField] private GameObject baseTilePrefab;
        [SerializeField] private GameObject borderTilePrefab;

        [Header("Materials")] [SerializeField] private Material baseMaterial;
        [SerializeField] private Material roadMaterial;

        #endregion

        #region Public Variables

        public int MapSize => mapSize;
        public float XOffset => xOffset;
        public float ZOffset => zOffset;
        public int RoadZ { get; private set; }

        public GameObject[,] MapTiles { get; private set; }

        public GameObject RoadSpawnPoint { get; private set; }

        public int BorderSize => borderSize;
        public bool IsMapCreated { get; private set; }

        public GameObject RoadToDestroy
        {
            set => _currentRoadToDestroy = value;
        }

        public NavMeshSurface NavMeshSurface { get; private set; }

        #endregion

        #region Private Variables

        private GameObject[,] _borderTiles;
        private GameObject _currentRoadToDestroy;
        private MeshRenderer[,] _mapTilesRenders;

        private enum Tile
        {
            baseTile,
            border,
            road
        }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            NavMeshSurface = roadParent.GetComponent<NavMeshSurface>();
        }

        #endregion

        #region Public Methods

        public void InitializeMap()
        {
            BuildMap();
            BuildBorder();
            BuildStartRoad();
            BuildDecoration();
            CameraController.Instance.StartingPosition();
            InhabitantsManager.Instance.RoadPosition = RoadSpawnPoint.transform.position;
            IsMapCreated = true;
        }

        public GameObject ChangeTileToRoad(int i, int j)
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.buildBuilding);
            _mapTilesRenders[i, j].material = roadMaterial;
            MapTiles[i, j].transform.SetParent(roadParent);
            SetTileName(Tile.road, i, j);
            NavMeshSurface.BuildNavMesh();
            return MapTiles[i, j];
        }

        public void DemolishBuilding(BuildManager.BuildingType type, GameObject building)
        {
            switch (type)
            {
                case BuildManager.BuildingType.house:
                case BuildManager.BuildingType.playground:
                case BuildManager.BuildingType.hospital:
                case BuildManager.BuildingType.police:
                    building.GetComponentInChildren<IBuilding>().Demolish();
                    building.GetComponent<BuildType>().type = BuildManager.BuildingType.none;
                    break;
                case BuildManager.BuildingType.road:
                    _currentRoadToDestroy = building;
                    DemolishRoad();
                    break;
                case BuildManager.BuildingType.none:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public void DemolishRoad()
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.demolishBuilding);
            ChangeRoadTileToBase((int)GetX_Index(_currentRoadToDestroy.transform.position.x), (int)GetZ_Index(_currentRoadToDestroy.transform.position.z));
            NavMeshSurface.UpdateNavMesh(NavMeshSurface.navMeshData);
        }

        public Vector2 TilePosition(float x, float z)
        {
            return new Vector2(GetX_Index(x), GetZ_Index(z));
        }

        #region Negihbours

        public List<GameObject> Get4Neighbours(GameObject tile, BuildManager.BuildingType type)
        {
            var neighbours = new List<GameObject>();
            var x = (int)GetX_Index(tile.transform.position.x);
            var z = (int)GetZ_Index(tile.transform.position.z);

            //Right
            //Debug.Log(_MapTiles[x + 1, z].name);
            if (x + 1 < MapSize && MapTiles[x + 1, z].GetComponent<BuildType>().type == type)
            {
                neighbours.Add(MapTiles[x + 1, z]);
            }

            //Left
            //Debug.Log(_MapTiles[x - 1, z].name);
            if (x - 1 >= 0 && MapTiles[x - 1, z].GetComponent<BuildType>().type == type)
            {
                neighbours.Add(MapTiles[x - 1, z]);
            }

            //Up
            //Debug.Log(_MapTiles[x, z + 1].name);
            if (z + 1 < MapSize && MapTiles[x, z + 1].GetComponent<BuildType>().type == type)
            {
                neighbours.Add(MapTiles[x, z + 1]);
            }

            //Down
            //Debug.Log(_MapTiles[x, z - 1].name);
            if (z - 1 >= 0 && MapTiles[x, z - 1].GetComponent<BuildType>().type == type)
            {
                neighbours.Add(MapTiles[x, z - 1]);
            }

            return neighbours;
        }

        public List<GameObject> Get4Neighbours(GameObject tile)
        {
            var neighbours = new List<GameObject>();
            var x = (int)GetX_Index(tile.transform.position.x);
            var z = (int)GetZ_Index(tile.transform.position.z);

            //Right
            neighbours.Add(x + 1 < MapSize ? MapTiles[x + 1, z] : null);
            //Left
            neighbours.Add(x - 1 >= 0 ? MapTiles[x - 1, z] : null);
            //Up
            neighbours.Add(z + 1 < MapSize ? MapTiles[x, z + 1] : null);
            //Down
            neighbours.Add(z - 1 >= 0 ? MapTiles[x, z - 1] : null);

            return neighbours;
        }

        public List<GameObject> Get8Neighbours(GameObject tile)
        {
            var neighbours = new List<GameObject>();
            var x = (int)GetX_Index(tile.transform.position.x);
            var z = (int)GetZ_Index(tile.transform.position.z);

            for (var i = x - 1; i <= x + 1; i++)
            {
                for (var j = z - 1; j <= z + 1; j++)
                {
                    if (i >= 0 && i < MapSize && j >= 0 && j < MapSize && MapTiles[i, j] != tile) neighbours.Add(MapTiles[i, j]);
                    else neighbours.Add(null);
                }
            }

            return neighbours;
        }

        public List<GameObject> Get12Neightbour(GameObject tile)
        {
            var neighbours = Get8Neighbours(tile);
            var x = (int)GetX_Index(tile.transform.position.x);
            var z = (int)GetZ_Index(tile.transform.position.z);
            //Right
            if (x + 2 < MapSize) neighbours.Add(MapTiles[x + 2, z]);
            else neighbours.Add(null);
            //Left
            if (x - 2 >= 0) neighbours.Add(MapTiles[x - 2, z]);
            else neighbours.Add(null);
            //Up
            if (z + 2 < MapSize) neighbours.Add(MapTiles[x, z + 2]);
            else neighbours.Add(null);
            //Down
            if (z - 2 >= 0) neighbours.Add(MapTiles[x, z - 2]);
            else neighbours.Add(null);

            return neighbours;
        }

        public List<GameObject> Get25Neighbour(GameObject tile)
        {
            var neighbours = new List<GameObject>();
            var x = (int)GetX_Index(tile.transform.position.x);
            var z = (int)GetZ_Index(tile.transform.position.z);

            for (var i = x - 2; i <= x + 2; i++)
            {
                for (var j = z - 2; j <= z + 2; j++)
                {
                    if (i >= 0 && i < MapSize && j >= 0 && j < MapSize && MapTiles[i, j] != tile) neighbours.Add(MapTiles[i, j]);
                    else neighbours.Add(null);
                }
            }

            return neighbours;
        }

        #endregion

        public void DestroyAllMap()
        {
            foreach (var mapTile in mapParent.GetComponentsInChildren<BuildType>())
            {
                DestroyImmediate(mapTile.gameObject);
            }

            foreach (var borderTile in borderParent.GetComponentsInChildren<BoxCollider>())
            {
                DestroyImmediate(borderTile.gameObject);
            }

            foreach (var roadTile in roadParent.GetComponentsInChildren<BuildType>())
            {
                DestroyImmediate(roadTile.gameObject);
            }
        }

        #endregion

        #region Private Methods

        private void BuildMap()
        {
            MapTiles = new GameObject[mapSize, mapSize];
            _mapTilesRenders = new MeshRenderer[mapSize, mapSize];
            for (var i = 0; i < mapSize; i++)
            {
                for (var j = 0; j < mapSize; j++)
                {
                    var newTile = BuildNewTile(i, j, Tile.baseTile, mapParent);
                    InicializateLineRenderer(newTile);
                    _mapTilesRenders[i, j] = newTile.GetComponentInChildren<MeshRenderer>();
                    MapTiles[i, j] = newTile;
                }
            }
        }

        private void BuildBorder()
        {
            _borderTiles = new GameObject[mapSize + 2 * borderSize, mapSize + 2 * borderSize];
            for (var i = -borderSize; i < mapSize + borderSize; i++)
            {
                for (var j = -borderSize; j < borderSize + mapSize; j++)
                {
                    if (TileBelongsToMap(i, j)) continue;
                    var newTile = BuildNewTile(i, j, Tile.border, borderParent);
                    _borderTiles[i + borderSize, j + borderSize] = newTile;
                }
            }
        }

        private void BuildStartRoad()
        {
            var positionY = Random.Range(3, mapSize - 3);
            RoadZ = positionY;
            for (var i = -borderSize; i < 0; i++)
            {
                var newTile = BuildNewTile(i, positionY, Tile.road, roadParent);
                if (i == -borderSize) RoadSpawnPoint = newTile;
                newTile.layer = 0;
                Destroy(_borderTiles[i + borderSize, positionY + borderSize]);
                _borderTiles[i + borderSize, positionY + borderSize] = newTile;
            }
        }

        private void BuildDecoration()
        {
            var maxDecoration = Random.Range(minNumDecoration, maxNumDecoration);
            var y = BuildManager.Instance.YBuilding;

            var borderTiles = borderParent.GetComponentsInChildren<Transform>(true);
            for (var i = 0; i < maxDecoration; i++)
            {
                var decorationGameObject = decorationGameObjectsList[Random.Range(0, decorationGameObjectsList.Count)];
                var currentBorderTile = borderTiles[Random.Range(0, borderTiles.Length)];

                while (currentBorderTile.GetComponentsInChildren<Transform>().Length > 3)
                {
                    currentBorderTile = borderTiles[Random.Range(0, borderTiles.Length)];
                }

                var newDecoration = Instantiate(decorationGameObject, currentBorderTile.transform);
                newDecoration.name = decorationGameObject.name + " : " + currentBorderTile.name;
                newDecoration.transform.position = new Vector3(newDecoration.transform.position.x, y, newDecoration.transform.position.z);
                var rotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
                newDecoration.transform.rotation = rotation;
            }
        }

        private bool TileBelongsToMap(int x, int y)
        {
            return (x >= 0 && y >= 0 && x < mapSize && y < mapSize);
        }

        private float GetX_Index(float x)
        {
            return x / XOffset;
        }

        private float GetZ_Index(float z)
        {
            return z / ZOffset;
        }

        private GameObject BuildNewTile(int i, int j, Tile type, Transform parent)
        {
            var position = new Vector3(i * xOffset, 0, j * zOffset);
            GameObject newTile;
            switch (type)
            {
                case Tile.baseTile:
                    newTile = Instantiate(baseTilePrefab, position, Quaternion.identity, parent);
                    break;
                case Tile.border:
                    newTile = Instantiate(borderTilePrefab, position, Quaternion.identity, parent);
                    break;
                case Tile.road:
                    newTile = Instantiate(baseTilePrefab, position, Quaternion.identity, parent);
                    newTile.GetComponentInChildren<MeshRenderer>().material = roadMaterial;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            newTile.name = type + "[" + i + ", " + j + "]";

            return newTile;
        }

        private void ChangeRoadTileToBase(int i, int j)
        {
            if (i == 0 && j == RoadZ) BuildManager.Instance.IsFirstRoadBuild = false;
            _mapTilesRenders[i, j].material = baseMaterial;
            MapTiles[i, j].GetComponent<BuildType>().type = BuildManager.BuildingType.none;
            MapTiles[i, j].transform.SetParent(mapParent);
            SetTileName(Tile.baseTile, i, j);
            UIManagerInGame.Instance.DisableAllHUDExceptBuildPanel();
        }

        private void SetTileName(Tile type, int i, int j)
        {
            MapTiles[i, j].name = type + "[" + i + ", " + j + "]";
        }

        private void InicializateLineRenderer(GameObject newTile)
        {
            var lineRenderer = newTile.GetComponent<LineRenderer>();
            var collider = newTile.GetComponent<Collider>();
            var maxCol = collider.bounds.max;
            var minCol = collider.bounds.min;
            var positions = new Vector3[4];
            positions[0] = transform.TransformPoint(new Vector3(minCol.x, maxCol.y, minCol.z));
            positions[1] = transform.TransformPoint(new Vector3(minCol.x, maxCol.y, maxCol.z));
            positions[2] = transform.TransformPoint(new Vector3(maxCol.x, maxCol.y, maxCol.z));
            positions[3] = transform.TransformPoint(new Vector3(maxCol.x, maxCol.y, minCol.z));
            lineRenderer.SetPositions(positions);
        }

        #endregion
    }
}