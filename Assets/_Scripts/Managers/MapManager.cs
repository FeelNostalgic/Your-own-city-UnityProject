using System;
using System.Collections.Generic;
using Controllers;
using Buildings;
using Commons;
using Utilities;
using Unity.AI.Navigation;
using UnityEngine;
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

        [SerializeField] [Range(2, 100)] private int minNumDecoration;

        [SerializeField] [Range(2, 100)] private int maxNumDecoration;

        [SerializeField] private List<GameObject> decorationGameObjectsList;

        [Header("Road")] [SerializeField] private Transform roadParent;

        [Header("Offsets")] [SerializeField] private float xOffset;

        [SerializeField] private float zOffset;

        [Header("Tiles")] [SerializeField] private GameObject baseTilePrefab;
        [SerializeField] private GameObject borderTilePrefab;

        [Header("Materials")] [SerializeField] private Material baseMaterial;
        [SerializeField] private Material roadMaterial;

        #endregion

        #region Public Variables

        public int MapSize => mapSize;
        public float XOffset => xOffset;
        public float ZOffset => zOffset;
        public static int RoadJ { get; private set; }


        public GameObject RoadSpawnPoint { get; private set; }

        public int BorderSize => borderSize;
        public bool IsMapCreated { get; private set; }

        #endregion

        #region Private Variables

        private static GameObject[,] _borderTiles;
        private GameObject[,] _mapTilesGameObjects;
        private static MeshRenderer[,] _mapTilesMeshRenderers;
        private static TileFunctionality[,] _mapTilesFunctionality;

        private NavMeshSurface _navMeshSurface;

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
            _navMeshSurface = GetComponentInChildren<NavMeshSurface>();
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

        public void ChangeTileToRoad(int i, int j)
        {
            _mapTilesMeshRenderers[i, j].material = roadMaterial;
            _mapTilesGameObjects[i, j].transform.SetParent(roadParent);
            SetTileName(Tile.road, i, j);
            _navMeshSurface.BuildNavMesh();
        }

        /// <summary>
        /// Set I,J tile map position to new building
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="building"></param>
        /// <param name="buildingType"></param>
        public static void SetTileToNewBuilding(int i, int j, Building building, BuildingType buildingType)
        {
            _mapTilesFunctionality[i, j].BuildingType = buildingType;
            _mapTilesFunctionality[i, j].Building = building;
        }

        public void DemolishBuilding(TileFunctionality tileToDemolish)
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.demolishBuilding);
            var (i, j) = tileToDemolish.MapPosition;

            switch (tileToDemolish.BuildingType)
            {
                case BuildingType.house:
                case BuildingType.playground:
                case BuildingType.hospital:
                case BuildingType.police:
                case BuildingType.road:
                    _mapTilesFunctionality[i, j].Building.Demolish();
                    SetTileToNewBuilding(i, j, null, BuildingType.none);
                    break;
                case BuildingType.none:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tileToDemolish.BuildingType), tileToDemolish.BuildingType, null);
            }
        }

        public void DemolishRoad(Vector3 positionInWorld)
        {
            var (i, j) = GetTilePositionInMap(positionInWorld.x, positionInWorld.z);
            ChangeRoadTileToBase(i, j);
            _navMeshSurface.UpdateNavMesh(_navMeshSurface.navMeshData);
        }

        /// <summary>
        /// Return a Vector2 with I,J tile map position
        /// </summary>
        /// <param name="x">Tile x position in world</param>
        /// <param name="z">Tile z position in world</param>
        /// <returns></returns>
        public Vector2Int GetTilePositionInMap(float x, float z)
        {
            return new Vector2Int(GetI(x), GetJ(z));
        }

        public TileFunctionality GetTile(Vector3 tilePosition)
        {
            var positionInMap = GetTilePositionInMap(tilePosition.x, tilePosition.z);
            return _mapTilesFunctionality[positionInMap.x, positionInMap.y];
        }

        public bool TryGetTile(Vector3 tilePosition, out TileFunctionality tile)
        {
           var (i,j) = GetTilePositionInMap(tilePosition.x, tilePosition.z);
            if (!TileBelongsToMap(i,j))
            {
                tile = null;
                return false;
            }

            tile = _mapTilesFunctionality[i, j];
            return true;
        }

        public bool TryGetTile(int i, int j, out TileFunctionality tile)
        {
            if (!TileBelongsToMap(new Vector2Int(i, j)))
            {
                tile = null;
                return false;
            }

            tile = _mapTilesFunctionality[i, j];
            return true;
        }

        #region Negihbours

        public List<TileFunctionality> Get4Neighbours(TileFunctionality tile, BuildingType type)
        {
            var neighbours = new List<TileFunctionality>();
            var i = GetI(tile.transform.position.x);
            var j = GetJ(tile.transform.position.z);

            //Right
            //Debug.Log(_MapTiles[x + 1, z].name);
            if (i + 1 < MapSize && _mapTilesFunctionality[i + 1, j].BuildingType == type)
            {
                neighbours.Add(_mapTilesFunctionality[i + 1, j]);
            }

            //Left
            //Debug.Log(_MapTiles[x - 1, z].name);
            if (i - 1 >= 0 && _mapTilesFunctionality[i - 1, j].BuildingType == type)
            {
                neighbours.Add(_mapTilesFunctionality[i - 1, j]);
            }

            //Up
            //Debug.Log(_MapTiles[x, z + 1].name);
            if (j + 1 < MapSize && _mapTilesFunctionality[i, j + 1].BuildingType == type)
            {
                neighbours.Add(_mapTilesFunctionality[i, j + 1]);
            }

            //Down
            //Debug.Log(_MapTiles[x, z - 1].name);
            if (j - 1 >= 0 && _mapTilesFunctionality[i, j - 1].BuildingType == type)
            {
                neighbours.Add(_mapTilesFunctionality[i, j - 1]);
            }

            return neighbours;
        }

        public List<TileFunctionality> Get4Neighbours(Vector3 tilePosition)
        {
            var neighbours = new List<TileFunctionality>();
            var tile = GetTile(tilePosition);
            var (i, j) = tile.MapPosition;

            //Right
            neighbours.Add(i + 1 < MapSize ? _mapTilesFunctionality[i + 1, j] : null);
            //Left
            neighbours.Add(i - 1 >= 0 ? _mapTilesFunctionality[i - 1, j] : null);
            //Up
            neighbours.Add(j + 1 < MapSize ? _mapTilesFunctionality[i, j + 1] : null);
            //Down
            neighbours.Add(j - 1 >= 0 ? _mapTilesFunctionality[i, j - 1] : null);

            return neighbours;
        }

        public List<TileFunctionality> Get8Neighbours(Vector3 tilePosition)
        {
            var neighbours = new List<TileFunctionality>();
            var tile = GetTile(tilePosition);
            var (i, j) = tile.MapPosition;

            for (var x = i - 1; x <= i + 1; x++)
            {
                for (var z = j - 1; z <= j + 1; z++)
                {
                    if (x >= 0 && x < MapSize && z >= 0 && z < MapSize && _mapTilesFunctionality[x, z] != tile) neighbours.Add(_mapTilesFunctionality[x, z]);
                    else neighbours.Add(null);
                }
            }

            return neighbours;
        }

        public List<TileFunctionality> Get12Neighbours(Vector3 tilePosition)
        {
            var neighbours = Get8Neighbours(tilePosition);
            var tile = GetTile(tilePosition);
            var i = GetI(tile.transform.position.x);
            var j = GetJ(tile.transform.position.z);

            //Right
            if (i + 2 < MapSize) neighbours.Add(_mapTilesFunctionality[i + 2, j]);
            else neighbours.Add(null);
            //Left
            if (i - 2 >= 0) neighbours.Add(_mapTilesFunctionality[i - 2, j]);
            else neighbours.Add(null);
            //Up
            if (j + 2 < MapSize) neighbours.Add(_mapTilesFunctionality[i, j + 2]);
            else neighbours.Add(null);
            //Down
            if (j - 2 >= 0) neighbours.Add(_mapTilesFunctionality[i, j - 2]);
            else neighbours.Add(null);

            return neighbours;
        }

        public List<TileFunctionality> Get25Neighbour(Vector3 tilePosition)
        {
            var tile = GetTile(tilePosition);
            var neighbours = new List<TileFunctionality>();
            var (i, j) = tile.MapPosition;

            for (var x = i - 2; x <= i + 2; x++)
            {
                for (var z = j - 2; z <= j + 2; z++)
                {
                    if (x >= 0 && x < MapSize && z >= 0 && z < MapSize && _mapTilesFunctionality[x, z] != tile) neighbours.Add(_mapTilesFunctionality[x, z]);
                    else neighbours.Add(null);
                }
            }

            return neighbours;
        }

        #endregion

        public void DestroyAllMap()
        {
            Helpers.DestroyChildren(mapParent);
            Helpers.DestroyChildren(borderParent);
            Helpers.DestroyChildren(roadParent);
        }

        #endregion

        #region Private Methods

        private void BuildMap()
        {
            InitializeMapArrays();
            for (var i = 0; i < mapSize; i++)
            {
                for (var j = 0; j < mapSize; j++)
                {
                    var newTile = BuildNewTile(i, j, Tile.baseTile, mapParent);
                    InicializateLineRenderer(newTile);
                    _mapTilesMeshRenderers[i, j] = newTile.GetComponentInChildren<MeshRenderer>();
                    _mapTilesFunctionality[i, j] = newTile.GetComponent<TileFunctionality>();
                    _mapTilesFunctionality[i, j].MapPosition = new Vector2Int(i, j);
                    _mapTilesGameObjects[i, j] = newTile;
                }
            }
        }

        private void InitializeMapArrays()
        {
            _mapTilesGameObjects = new GameObject[mapSize, mapSize];
            _mapTilesMeshRenderers = new MeshRenderer[mapSize, mapSize];
            _mapTilesFunctionality = new TileFunctionality[mapSize, mapSize];
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
            RoadJ = positionY;
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

        private bool TileBelongsToMap(Vector2Int tile)
        {
            return TileBelongsToMap(tile.x, tile.y);
        }

        private bool TileBelongsToMap(int i, int j)
        {
            return i >= 0 && j >= 0 && i < mapSize && j < mapSize;
        }

        /// <summary>
        /// World position to I map
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private int GetI(float x)
        {
            var result = x / xOffset;
            // Debug.Log($"Round: {x} / {xOffset} = {result} ((int) {Mathf.RoundToInt(result)})");
            return Mathf.RoundToInt(result);
        }

        /// <summary>
        /// World position to J map
        /// </summary>
        /// <param name="z"></param>
        /// <returns></returns>
        private int GetJ(float z)
        {
            var result = z / ZOffset;
            // Debug.Log($"{z} / {zOffset} = {result} ((int) {Mathf.RoundToInt(result)})");
            return Mathf.RoundToInt(result);
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
                    // Build roadFunctionality
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            newTile.name = type + "[" + i + ", " + j + "]";

            return newTile;
        }

        private void ChangeRoadTileToBase(int i, int j)
        {
            if (i == 0 && j == RoadJ) BuildManager.Instance.IsFirstRoadBuild = false;
            _mapTilesMeshRenderers[i, j].material = baseMaterial;
            _mapTilesGameObjects[i, j].transform.SetParent(mapParent);
            SetTileName(Tile.baseTile, i, j);
        }

        private void SetTileName(Tile type, int i, int j)
        {
            _mapTilesGameObjects[i, j].name = type + "[" + i + ", " + j + "]";
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