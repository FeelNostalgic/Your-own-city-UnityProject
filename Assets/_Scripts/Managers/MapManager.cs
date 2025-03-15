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
        public float X_Offset => x_Offset;
        public float Z_Offset => z_Offset;
        public int RoadZ => _roadZ;
        public GameObject[,] MapTiles => _MapTiles;
        public Transform RoadParent => roadParent;
        public GameObject RoadSpawnPoint => _roadSpawnPoint;
        public int BorderSize => borderSize;
        public bool IsMapCreated => _isMapCreated;
        public GameObject RoadToDestroy
        {
            set => _currentRoadToDestroy = value;
        }
        
        #endregion

        #region Private Variables

        private GameObject[,] _MapTiles;
        private GameObject[,] _BorderTiles;
        private int _roadZ;
        private GameObject _currentRoadToDestroy;
        private GameObject _roadSpawnPoint;
        private bool _isMapCreated;
        
        private enum _tile
        {
            baseT, border, road
        }

        #endregion

        #region Unity Methods

        private void Start()
        {
            _isMapCreated = false;
        }

        #endregion

        #region Public Methods

        public void StartMap()
        {
            BuildMap();
            BuildBorder();
            BuildStartRoad();
            BuildDecoration();
            CameraController.Instance.StartingPosition();
            ResidentsManager.Instance.RoadPosition = _roadSpawnPoint.transform.position;
            _isMapCreated = true;
        }
        
        public GameObject BuildRoadAtMap(int i, int j)
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.buildBuilding);
            Destroy(_MapTiles[i,j]);
            var newtile = BuildTile(i, j, _tile.road, roadParent);
            InicializateLineRenderer(newtile);
            _MapTiles[i, j] = newtile;
            return newtile;
        }

        public void DestroyRoad()
        {
            DestroyRoadAtMap((int)GetX_Index(_currentRoadToDestroy.transform.position.x), (int)GetZ_Index(_currentRoadToDestroy.transform.position.z));
            RoadParent.GetComponent<NavMeshSurface>().UpdateNavMesh(RoadParent.GetComponent<NavMeshSurface>().navMeshData);
        }

        public Vector2 TilePosition(float x, float z)
        {
            return new Vector2(GetX_Index(x), GetZ_Index(z));
        }

        #region GetVecinos
        public List<GameObject> GetVecinos4(GameObject tile, BuildManager._building type)
        {
            var vecinos = new List<GameObject>();
            var x = (int)GetX_Index(tile.transform.position.x);
            var z = (int)GetZ_Index(tile.transform.position.z);
            
            //Derecha
            //Debug.Log(_MapTiles[x + 1, z].name);
            if ((x+1) < MapSize && _MapTiles[x + 1, z].GetComponent<BuildType>().Type == type)
            {
                vecinos.Add(_MapTiles[x + 1, z]);
            }
            //Izquierda
            //Debug.Log(_MapTiles[x - 1, z].name);
            if ((x-1) >= 0 && _MapTiles[x - 1, z].GetComponent<BuildType>().Type == type)
            {
                vecinos.Add(_MapTiles[x - 1, z]);
            }
            //Up
            //Debug.Log(_MapTiles[x, z + 1].name);
            if ((z+1) < MapSize && _MapTiles[x, z + 1].GetComponent<BuildType>().Type == type)
            {
                vecinos.Add(_MapTiles[x, z + 1]);
            }
            //Down
            //Debug.Log(_MapTiles[x, z - 1].name);
            if ((z-1) >= 0 && _MapTiles[x, z - 1].GetComponent<BuildType>().Type == type)
            {
                vecinos.Add(_MapTiles[x, z - 1]);
            }
            
            return vecinos;
        }
        
        public List<GameObject> GetVecinos4(GameObject tile)
        {
            var vecinos = new List<GameObject>();
            var x = (int)GetX_Index(tile.transform.position.x);
            var z = (int)GetZ_Index(tile.transform.position.z);
            
            //Derecha
            if ((x+1) < MapSize) vecinos.Add(_MapTiles[x + 1, z]);
            else vecinos.Add(null);
            //Izquierda
            if ((x-1) >= 0) vecinos.Add(_MapTiles[x - 1, z]);
            else vecinos.Add(null);
            //Up
            if ((z+1) < MapSize) vecinos.Add(_MapTiles[x, z + 1]);
            else vecinos.Add(null);
            //Down
            if ((z-1) >= 0) vecinos.Add(_MapTiles[x, z - 1]);
            else vecinos.Add(null);
            
            return vecinos;
        }

        public List<GameObject> GetVecinos8(GameObject tile)
        {
            var vecinos = new List<GameObject>();
            var x = (int)GetX_Index(tile.transform.position.x);
            var z = (int)GetZ_Index(tile.transform.position.z);
            
            for (int i = x-1; i <= x+1; i++)
            {
                for (int j = z-1; j <= z+1; j++)
                {
                    if (i >= 0 && i < MapSize && j >= 0 && j < MapSize && _MapTiles[i,j] != tile) vecinos.Add(_MapTiles[i,j]);
                    else vecinos.Add(null);
                }
            }
            
            return vecinos;
        }

        public List<GameObject> GetVecinos12(GameObject tile)
        {
            var vecinos = GetVecinos8(tile);
            var x = (int)GetX_Index(tile.transform.position.x);
            var z = (int)GetZ_Index(tile.transform.position.z);
            //Derecha
            if ((x+2) < MapSize)vecinos.Add(_MapTiles[x + 2, z]);
            else vecinos.Add(null);
            //Izquierda
            if ((x-2) >= 0) vecinos.Add(_MapTiles[x - 2, z]);
            else vecinos.Add(null);
            //Up
            if ((z+2) < MapSize) vecinos.Add(_MapTiles[x, z + 2]);
            else vecinos.Add(null);
            //Down
            if ((z-2) >= 0) vecinos.Add(_MapTiles[x, z - 2]);
            else vecinos.Add(null);
            
            return vecinos;
        }

        public List<GameObject> GetVecinos25(GameObject tile)
        {
            var vecinos = new List<GameObject>();
            var x = (int)GetX_Index(tile.transform.position.x);
            var z = (int)GetZ_Index(tile.transform.position.z);
            
            for (int i = x-2; i <= x+2; i++)
            {
                for (int j = z-2; j <= z+2; j++)
                {
                    if (i >= 0 && i < MapSize && j >= 0 && j < MapSize && _MapTiles[i,j] != tile) vecinos.Add(_MapTiles[i,j]);
                    else vecinos.Add(null);
                }
            }
            
            return vecinos;
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
            _MapTiles = new GameObject[mapSize, mapSize];
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    var newtile = BuildTile(i,j,_tile.baseT, mapParent);
                    InicializateLineRenderer(newtile);
                    _MapTiles[i, j] = newtile;
                }
            }
        }

        private void BuildBorder()
        {
            _BorderTiles = new GameObject[mapSize + 2 * borderSize, mapSize + 2 * borderSize];
            for (int i = -borderSize; i < mapSize + borderSize; i++)
            {
                for (int j = -borderSize; j < borderSize + mapSize; j++)
                {
                    if (checkPertenenciaMap(i,j))
                    {
                        var newtile = BuildTile(i, j, _tile.border, borderParent);
                        _BorderTiles[i + borderSize, j + borderSize] = newtile;
                    }
                }
            }
        }
        
        private void BuildStartRoad()
        {
            var positionY = Random.Range(3, mapSize-3);
            _roadZ = positionY;
            for (int i = -borderSize; i < 0; i++)
            {
                var newtile = BuildTile(i, positionY, _tile.road, roadParent);
                if (i == -borderSize) _roadSpawnPoint = newtile;
                newtile.layer = 0;
                Destroy(_BorderTiles[i + borderSize,positionY + borderSize]);
                _BorderTiles[i + borderSize, positionY + borderSize] = newtile;
            }
        }

        private void BuildDecoration()
        {
            var maxDecoration = Random.Range(MinNumDecoration, MaxNumDecoration);
            var y = BuildManager.Instance.YBuilding;

            var borderTiles = borderParent.GetComponentsInChildren<BoxCollider>(true);
            for (int i = 0; i < maxDecoration; i++)
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
            return (x / X_Offset);
        }
        
        private float GetZ_Index(float z)
        {
            return (z / Z_Offset);
        }
        
        private GameObject BuildTile(int i, int j, _tile type, Transform parent)
        {
            var position = new Vector3(i * x_Offset, 0, j * z_Offset);
            var newtile = Instantiate(tiles[(int)type], position, Quaternion.identity, parent);
            newtile.name = type + "[" + i + ", " + j + "]";

            return newtile;
        }
        
        private void DestroyRoadAtMap(int i, int j)
        {
            if (i == 0 && j == RoadZ) BuildManager.Instance.IsFirstRoadBuild = false;
            Destroy(_MapTiles[i,j]);
            var newtile = BuildTile(i, j, _tile.baseT, mapParent);
            InicializateLineRenderer(newtile);
            _MapTiles[i, j] = newtile;
            UIManager.Instance.DisableAllPanels();
        }

        private void InicializateLineRenderer(GameObject newtile)
        {
            var lineRenderer = newtile.GetComponent<LineRenderer>();
            var collider = newtile.GetComponent<Collider>();
            var maxCol = collider.bounds.max;
            var minCol = collider.bounds.min;
            Vector3[] positions = new Vector3[4];
            positions[0] = transform.TransformPoint( new Vector3(minCol.x, maxCol.y,minCol.z));
            positions[1] = transform.TransformPoint(new Vector3(minCol.x, maxCol.y,maxCol.z));
            positions[2] = transform.TransformPoint(new Vector3(maxCol.x, maxCol.y,maxCol.z));
            positions[3] = transform.TransformPoint(new Vector3(maxCol.x, maxCol.y,minCol.z));
            lineRenderer.SetPositions(positions);
        }
        
        #endregion
    }
}