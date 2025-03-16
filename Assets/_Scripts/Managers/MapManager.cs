using System;
using System.Collections.Generic;
using Controllers;
using Buildings;
using Utilities;
using Unity.AI.Navigation;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public class MapManager : MonoBehaviourSinglenton<MapManager>
    {
        #region Inspector Variables
        
        [Header("MapTile")]
        [Tooltip("Genera un mapa de MapSizexMapSize")] [Range(8, 100)] [SerializeField]
        private int mapSize;
        [SerializeField] private Transform mapParent;
        
        [Header("Border")]
        [Tooltip("Crea un borde exterior al mapa principal con este anchura")] [Range(8, 25)] [SerializeField]
        private int borderSize;
        [SerializeField] private Transform borderParent;
        [SerializeField] [Range(2, 100)] private int MinNumDecoration;
        [SerializeField] [Range(2, 100)] private int MaxNumDecoration;
        [SerializeField] private List<GameObject> decorationGameObjectsList;

        [Header("Road")]
        [SerializeField] private Transform roadParent;
        
        [Header("Offsets")]
        [SerializeField] private float x_Offset;
        [SerializeField] private float z_Offset;

        [Header("Tiles Prefabs")]
        [Tooltip("Orden: Base, Border, Road")]
        [SerializeField] private GameObject[] tiles;
        
        
        #endregion

        #region Public Variables

        public int MapSize => mapSize;
        public float XOffset => x_Offset;
        public float ZOffset => z_Offset;
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

        public static event Action<GameObject> OnTileDelete;
        
        #endregion

        #region Private Variables

        private GameObject[,] _borderTiles;
        private GameObject _currentRoadToDestroy;

        private enum Tile
        {
            baseT, border, road
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
            ResidentsManager.Instance.RoadPosition = RoadSpawnPoint.transform.position;
            IsMapCreated = true;
        }
        
        public GameObject BuildRoadAtMap(int i, int j)
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.buildBuilding);
            OnTileDelete?.Invoke(MapTiles[i,j]);
            Destroy(MapTiles[i,j]);
            var newTile = BuildTile(i, j, Tile.road, roadParent);
            InicializateLineRenderer(newTile);
            MapTiles[i, j] = newTile;
            return newTile;
        }

        public void DestroyRoad()
        {
            DestroyRoadAtMap((int)GetX_Index(_currentRoadToDestroy.transform.position.x), (int)GetZ_Index(_currentRoadToDestroy.transform.position.z));
            NavMeshSurface.UpdateNavMesh(NavMeshSurface.navMeshData);
        }

        public Vector2 TilePosition(float x, float z)
        {
            return new Vector2(GetX_Index(x), GetZ_Index(z));
        }

        #region GetVecinos
        public List<GameObject> Get4Neighbours(GameObject tile, BuildManager.BuildingType type)
        {
            var neightbour = new List<GameObject>();
            var x = (int)GetX_Index(tile.transform.position.x);
            var z = (int)GetZ_Index(tile.transform.position.z);
            
            //Right
            //Debug.Log(_MapTiles[x + 1, z].name);
            if ((x+1) < MapSize && MapTiles[x + 1, z].GetComponent<BuildType>().type == type)
            {
                neightbour.Add(MapTiles[x + 1, z]);
            }
            //Left
            //Debug.Log(_MapTiles[x - 1, z].name);
            if ((x-1) >= 0 && MapTiles[x - 1, z].GetComponent<BuildType>().type == type)
            {
                neightbour.Add(MapTiles[x - 1, z]);
            }
            //Up
            //Debug.Log(_MapTiles[x, z + 1].name);
            if ((z+1) < MapSize && MapTiles[x, z + 1].GetComponent<BuildType>().type == type)
            {
                neightbour.Add(MapTiles[x, z + 1]);
            }
            //Down
            //Debug.Log(_MapTiles[x, z - 1].name);
            if ((z-1) >= 0 && MapTiles[x, z - 1].GetComponent<BuildType>().type == type)
            {
                neightbour.Add(MapTiles[x, z - 1]);
            }
            
            return neightbour;
        }
        
        public List<GameObject> Get4Neighbours(GameObject tile)
        {
            var neightbours = new List<GameObject>();
            var x = (int)GetX_Index(tile.transform.position.x);
            var z = (int)GetZ_Index(tile.transform.position.z);
            
            //Right
            if ((x+1) < MapSize) neightbours.Add(MapTiles[x + 1, z]);
            else neightbours.Add(null);
            //Left
            if ((x-1) >= 0) neightbours.Add(MapTiles[x - 1, z]);
            else neightbours.Add(null);
            //Up
            if ((z+1) < MapSize) neightbours.Add(MapTiles[x, z + 1]);
            else neightbours.Add(null);
            //Down
            if ((z-1) >= 0) neightbours.Add(MapTiles[x, z - 1]);
            else neightbours.Add(null);
            
            return neightbours;
        }

        public List<GameObject> GetVecinos8(GameObject tile)
        {
            var neightbours = new List<GameObject>();
            var x = (int)GetX_Index(tile.transform.position.x);
            var z = (int)GetZ_Index(tile.transform.position.z);
            
            for (var i = x-1; i <= x+1; i++)
            {
                for (var j = z-1; j <= z+1; j++)
                {
                    if (i >= 0 && i < MapSize && j >= 0 && j < MapSize && MapTiles[i,j] != tile) neightbours.Add(MapTiles[i,j]);
                    else neightbours.Add(null);
                }
            }
            
            return neightbours;
        }

        public List<GameObject> GetVecinos12(GameObject tile)
        {
            var neightbours = GetVecinos8(tile);
            var x = (int)GetX_Index(tile.transform.position.x);
            var z = (int)GetZ_Index(tile.transform.position.z);
            //Right
            if ((x+2) < MapSize)neightbours.Add(MapTiles[x + 2, z]);
            else neightbours.Add(null);
            //Left
            if ((x-2) >= 0) neightbours.Add(MapTiles[x - 2, z]);
            else neightbours.Add(null);
            //Up
            if ((z+2) < MapSize) neightbours.Add(MapTiles[x, z + 2]);
            else neightbours.Add(null);
            //Down
            if ((z-2) >= 0) neightbours.Add(MapTiles[x, z - 2]);
            else neightbours.Add(null);
            
            return neightbours;
        }

        public List<GameObject> GetVecinos25(GameObject tile)
        {
            var neightbours = new List<GameObject>();
            var x = (int)GetX_Index(tile.transform.position.x);
            var z = (int)GetZ_Index(tile.transform.position.z);
            
            for (var i = x-2; i <= x+2; i++)
            {
                for (var j = z-2; j <= z+2; j++)
                {
                    if (i >= 0 && i < MapSize && j >= 0 && j < MapSize && MapTiles[i,j] != tile) neightbours.Add(MapTiles[i,j]);
                    else neightbours.Add(null);
                }
            }
            
            return neightbours;
        }
        #endregion

        public void DestroyAll()
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
            for (var i = 0; i < mapSize; i++)
            {
                for (var j = 0; j < mapSize; j++)
                {
                    var newtile = BuildTile(i,j,Tile.baseT, mapParent);
                    InicializateLineRenderer(newtile);
                    MapTiles[i, j] = newtile;
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
                    if (checkPertenenciaMap(i,j))
                    {
                        var newtile = BuildTile(i, j, Tile.border, borderParent);
                        _borderTiles[i + borderSize, j + borderSize] = newtile;
                    }
                }
            }
        }
        
        private void BuildStartRoad()
        {
            var positionY = Random.Range(3, mapSize-3);
            RoadZ = positionY;
            for (var i = -borderSize; i < 0; i++)
            {
                var newtile = BuildTile(i, positionY, Tile.road, roadParent);
                if (i == -borderSize) RoadSpawnPoint = newtile;
                newtile.layer = 0;
                Destroy(_borderTiles[i + borderSize,positionY + borderSize]);
                _borderTiles[i + borderSize, positionY + borderSize] = newtile;
            }
        }

        private void BuildDecoration()
        {
            var maxDecoration = Random.Range(MinNumDecoration, MaxNumDecoration);
            var y = BuildManager.Instance.YBuilding;

            var borderTiles = borderParent.GetComponentsInChildren<BoxCollider>(true);
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
                var rotation = Quaternion.Euler(new Vector3(0, Random.Range(0,360), 0));
                newDecoration.transform.rotation = rotation;
            }
        }
        
        private bool checkPertenenciaMap(int x, int y)
        {
            return !(x >= 0 && y >= 0 && x < mapSize && y < mapSize);
        }

        private float GetX_Index(float x)
        {
            return (x / XOffset);
        }
        
        private float GetZ_Index(float z)
        {
            return (z / ZOffset);
        }
        
        private GameObject BuildTile(int i, int j, Tile type, Transform parent)
        {
            var position = new Vector3(i * x_Offset, 0, j * z_Offset);
            var newtile = Instantiate(tiles[(int)type], position, Quaternion.identity, parent);
            newtile.name = type + "[" + i + ", " + j + "]";

            return newtile;
        }
        
        private void DestroyRoadAtMap(int i, int j)
        {
            if (i == 0 && j == RoadZ) BuildManager.Instance.IsFirstRoadBuild = false;
            OnTileDelete?.Invoke(MapTiles[i,j]);
            Destroy(MapTiles[i,j]);
            var newTile = BuildTile(i, j, Tile.baseT, mapParent);
            InicializateLineRenderer(newTile);
            MapTiles[i, j] = newTile;
            UIManagerInGame.Instance.DisableAllPanels();
        }

        private void InicializateLineRenderer(GameObject newTile)
        {
            var lineRenderer = newTile.GetComponent<LineRenderer>();
            var collider = newTile.GetComponent<Collider>();
            var maxCol = collider.bounds.max;
            var minCol = collider.bounds.min;
            var positions = new Vector3[4];
            positions[0] = transform.TransformPoint( new Vector3(minCol.x, maxCol.y,minCol.z));
            positions[1] = transform.TransformPoint(new Vector3(minCol.x, maxCol.y,maxCol.z));
            positions[2] = transform.TransformPoint(new Vector3(maxCol.x, maxCol.y,maxCol.z));
            positions[3] = transform.TransformPoint(new Vector3(maxCol.x, maxCol.y,minCol.z));
            lineRenderer.SetPositions(positions);
        }
        
        #endregion
    }
}