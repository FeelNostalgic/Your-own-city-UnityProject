using Managers;
using UnityEngine;

namespace Utilities
{
    public class CicloDiaNoche : MonoBehaviour
    {
        #region Inspector Variables

        [SerializeField] private float DuracionDiaEnMinutos;

        #endregion

        #region Public Variables

        #endregion

        #region Private Variables

        private float _SpeedAngle;
        private Quaternion _startRotation;
        
        #endregion

        #region Unity Methods

        private void Start()
        {
            _SpeedAngle = 360 / ((DuracionDiaEnMinutos * 2) * 60);
            _startRotation = transform.rotation;
        }

        private void Update()
        {
            if (!PointAndClickManager.Instance.IsGameOver && !PointAndClickManager.Instance.IsGamePaused)
            {
                transform.Rotate(Vector3.right * _SpeedAngle * Time.deltaTime);
            }

            if (PointAndClickManager.Instance.IsGameOver)
            {
                transform.rotation = _startRotation;
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion
    }
}