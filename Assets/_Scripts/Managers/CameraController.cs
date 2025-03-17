using Managers;
using Utilities;
using UnityEngine;

namespace Controllers
{
    public class CameraController : MonoBehaviourSinglenton<CameraController>
    {
        #region Inspector Variables

        [Header("Move")] [SerializeField] private float MoveSpeed;

        [Tooltip("El primer valor es el valor mínimo de Y, el segundo valor es el máximo de Y")] [SerializeField]
        private Vector2 LimitY;

        [Header("Rotation")] [SerializeField] private float RotationSpeed;
        [SerializeField] private float LimitRotation;

        [Header("Starting Position")] [SerializeField]
        private float AlturaOffset;

        [SerializeField] private float CasillasOffset;
        [SerializeField] private float AnguloInicial;

        #endregion

        #region Public Variables

        //EMPTY

        #endregion

        #region Private Variables

        private bool _canNavigate;
        private float _rotationY;
        private float _rotationX;
        private Camera _camera;
        
        #endregion

        #region Unity Methods
        
        private void Awake()
        {
            _rotationY = 0;
        }
        
        private void Start()
        {
            _camera = Helpers.Camera;
        }

        private void Update()
        {
            if (!MapManager.Instance.IsMapCreated || GameManager.CurrentGameState == GameState.Paused) return;
            
            ShowMouseCursor();
            NavigateMap();
        }

        #endregion

        #region Public Methods

        public void StartingPosition()
        {
            var firstRoad = MapManager.Instance.RoadSpawnPoint.transform.position;
            var position = new Vector3(firstRoad.x + (MapManager.Instance.BorderSize * MapManager.Instance.XOffset),
                firstRoad.y + (LimitY.x + AlturaOffset), firstRoad.z - (CasillasOffset * MapManager.Instance.ZOffset));
            transform.position = position;
            _camera.transform.rotation = Quaternion.Euler(new Vector3(AnguloInicial, 0, 0));
        }

        #endregion

        #region Private Methods

        private void ShowMouseCursor()
        {
            if (Input.GetMouseButtonDown(1))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                PointAndClickManager.DisableCurrentLineRendererSelected();
                UIManagerInGame.Instance.DisableAllPanels();
                _canNavigate = true;
            }

            if (!Input.GetMouseButtonUp(1)) return;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            _canNavigate = false;
        }

        private void NavigateMap()
        {
            if (!_canNavigate) return;
            CalculateRotation();
            CalculateMove();
        }

        private void CalculateRotation()
        {
            _rotationY += -Input.GetAxis("Mouse Y") * RotationSpeed;
            var rotationX = Input.GetAxis("Mouse X") * RotationSpeed;
            _rotationY = Mathf.Clamp(_rotationY, -LimitRotation, LimitRotation);
            _camera.transform.localRotation = Quaternion.Euler(_rotationY, 0, 0);
            transform.rotation *= Quaternion.Euler(0, rotationX, 0);
        }

        private void CalculateMove()
        {
            var h = Input.GetAxis("Horizontal");
            var v = Input.GetAxis("Vertical");

            transform.position += _camera.transform.forward * (v * MoveSpeed * Time.deltaTime);
            transform.position += Quaternion.AngleAxis(90, Vector3.up) * transform.forward *
                                  (h * MoveSpeed * Time.deltaTime);

            var vectorRightForward = Quaternion.AngleAxis(90, Vector3.up) * transform.forward;
            if (Input.GetKey(KeyCode.E)) //UP
            {
                transform.position += Quaternion.AngleAxis(-90, vectorRightForward) * transform.forward *
                                      (MoveSpeed * .8f * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.Q)) //DOWN
            {
                transform.position += Quaternion.AngleAxis(90, vectorRightForward) * transform.forward *
                                      (MoveSpeed * .8f * Time.deltaTime);
            }

            CalculateLimits();
        }

        private void CalculateLimits()
        {
            var x = MapManager.Instance.XOffset;
            var z = MapManager.Instance.ZOffset;
            var mapSize = MapManager.Instance.MapSize + 1;
            var borderHalfSize = (MapManager.Instance.BorderSize / 2) - 1;

            transform.LimitX(-x * borderHalfSize, x * (mapSize + borderHalfSize));
            transform.LimitY(LimitY.x, LimitY.y);
            transform.LimitZ(-z * borderHalfSize, z * (mapSize + borderHalfSize));
        }

        #endregion
    }
}