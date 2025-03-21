using Commons;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Utilities
{
    public class DayNightCycle : MonoBehaviour
    {
        #region Inspector Variables

        [FormerlySerializedAs("DuracionDiaEnMinutos")] [SerializeField] private float dayDurationInMinutes;

        #endregion

        #region Public Variables
        // EMPTY
        #endregion

        #region Private Variables

        private float _speedAngle;
        private Quaternion _startRotation;
        
        #endregion

        #region Unity Methods

        private void Start()
        {
            _speedAngle = 360 / ((dayDurationInMinutes * 2) * 60);
            _startRotation = transform.rotation;
            GameManager.OnAfterStateChange += ResetRotation;
        }
        
        private void Update()
        {
            if (GameManager.CurrentGameState == GameState.Playing)
            {
                transform.Rotate(Vector3.right * (_speedAngle * Time.deltaTime));
            }
        }

        #endregion

        #region Public Methods
        // EMPTY
        #endregion

        #region Private Methods

        private void ResetRotation(GameState newState)
        {
            if(newState == GameState.Starting) transform.rotation = _startRotation;
        }
        
        #endregion
    }
}