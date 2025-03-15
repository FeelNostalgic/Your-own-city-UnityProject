using Managers;
using Utilities;
using UnityEngine;

namespace Controllers
{
    public class CameraController : MonoBehaviourSinglenton<CameraController>
    {
        #region Inspector Variables

        [Header("Move")] [SerializeField] private float MoveSpeed;

        [Tooltip("El primer valor es el valor mínimo de Y, el segundo valor es el máximo de Y")] 
        [SerializeField] private Vector2 LimitY;

        [Header("Rotation")]
        [SerializeField] private float RotationSpeed;
        [SerializeField] private float LimitRotation;

        [Header("Starting Position")]
        [SerializeField] private float AlturaOffset;

        [SerializeField] private float CasillasOffset;
        [SerializeField] private float AnguloInicial;

        #endregion

        #region Public Variables

        #endregion

        #region Private Variables

        private bool _canNavigate;
        private float _RotationY;
        private float _RotationX;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _RotationY = 0;
        }

        private void Update()
        {
            if (MapManager.Instance.IsMapCreated && !PointAndClickManager.Instance.IsGamePaused)
            {
                ShowMouseCursor();
                NavigateMap();
            }
        }

        #endregion

        #region Public Methods

        public void StartingPosition()
        {
            var firstRoad = MapManager.Instance.RoadSpawnPoint.transform.position;
            var position = new Vector3(firstRoad.x + (MapManager.Instance.BorderSize * MapManager.Instance.X_Offset),
                firstRoad.y + (LimitY.x + AlturaOffset), firstRoad.z - (CasillasOffset * MapManager.Instance.Z_Offset));
            transform.position = position;
            UnityEngine.Camera.main.transform.rotation = Quaternion.Euler(new Vector3(AnguloInicial, 0, 0));
        }

        #endregion

        #region Private Methods

        private void ShowMouseCursor()
        {
            if (Input.GetMouseButtonDown(1))
            {
                Cursor.lockState = CursorLockMode.Locked;
                PointAndClickManager.Instance.DisableCurrentLineRenderer();
                Cursor.visible = true;
                _canNavigate = true;
            }

            if (Input.GetMouseButtonUp(1))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                _canNavigate = false;
            }
        }

        private void NavigateMap()
        {
            if (_canNavigate)
            {
                calculateRotation();
                calculateMove();
            }
        }
        
        private void calculateRotation()
        {
            _RotationY += -Input.GetAxis("Mouse Y") * RotationSpeed;
            float rotationX = Input.GetAxis("Mouse X") * RotationSpeed;
            _RotationY = Mathf.Clamp(_RotationY, -LimitRotation, LimitRotation);
            UnityEngine.Camera.main.transform.localRotation = Quaternion.Euler(_RotationY, 0, 0);
            transform.rotation *= Quaternion.Euler(0, rotationX, 0);
        }

        private void calculateMove()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            transform.position += UnityEngine.Camera.main.transform.forward * (v * MoveSpeed * Time.deltaTime);
            transform.position += Quaternion.AngleAxis(90, Vector3.up) * transform.forward *
                                  (h * MoveSpeed * Time.deltaTime);

            Vector3 vectorRightForward = Quaternion.AngleAxis(90, Vector3.up) * transform.forward;
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

            calculateLimits();
        }

        private void calculateLimits()
        {
            var X = MapManager.Instance.X_Offset;
            var Z = MapManager.Instance.Z_Offset;
            var mapSize = MapManager.Instance.MapSize + 1;
            var borderSizehalf = (MapManager.Instance.BorderSize / 2) - 1;

            transform.LimitX(-X * borderSizehalf, X * (mapSize + borderSizehalf));
            transform.LimitY(LimitY.x, LimitY.y);
            transform.LimitZ(-Z * borderSizehalf, Z * (mapSize + borderSizehalf));
        }

        #endregion
    }
}