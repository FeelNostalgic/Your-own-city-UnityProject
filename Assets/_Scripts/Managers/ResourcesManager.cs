using System;
using Commons;
using Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Managers
{
    public class ResourcesManager : MonoBehaviourSingleton<ResourcesManager>
    {
        #region Inspector Variables

        [FormerlySerializedAs("StartGold")] [SerializeField] private int startGold;

        #endregion

        #region Public Variables

        public static int CurrentGold { get; private set; }

        public static event Action<int> OnGoldUpdate;
        public static event Action<int> OnResidentUpdate;
        public static event Action<int> OnCostsPerSecondUpdate;
        public static event Action<int> OnGoldPerSecondUpdate;

        #endregion

        #region Private Variables

        private static int _currentInhabitants;
        private static int _currentCostsPerSecond;
        private static float _currentGoldPerSecond;
        
        #endregion

        #region Unity Methods

        private void Awake()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
        }

        private void Start()
        {
            AddGold(0);
            AddGold(startGold);
            AddResident(0);
            AddCosts(0);
            AddGoldPerSecond(0);
        }

        private void Update()
        {
            //Every second costs are reduced
            if (Time.frameCount % Application.targetFrameRate == 0 && GameManager.CurrentGameState == GameState.Playing)
            {
                AddGold(-_currentCostsPerSecond);
            }

            if (CurrentGold < -500 && GameManager.CurrentGameState == GameState.Playing)
            {
                GameOver();
            }
        }

        #endregion

        #region Public Methods

        public static void AddResident(int h)
        {
            _currentInhabitants += h;
            OnResidentUpdate?.Invoke(_currentInhabitants);
        }

        public static void AddGold(int g)
        {
            CurrentGold += g;
            OnGoldUpdate?.Invoke(CurrentGold);
        }

        public static void AddCosts(int g)
        {
            _currentCostsPerSecond += g;
            OnCostsPerSecondUpdate?.Invoke(_currentCostsPerSecond);
        }

        public static void AddGoldPerSecond(float g)
        {
            _currentGoldPerSecond += g;
            OnGoldPerSecondUpdate?.Invoke((int)_currentGoldPerSecond);
        }

        public void RestartAllInfo()
        {
            AddGold(-CurrentGold);
            AddGold(startGold);
            AddResident(-_currentInhabitants);
            AddGoldPerSecond(-_currentGoldPerSecond);
            AddCosts(-_currentCostsPerSecond);
        }

        #endregion

        #region Private Methods

        private static void GameOver()
        {
            GameManager.Instance.ChangeState(GameState.GameOver);
            Time.timeScale = 0;
            UIManagerInGame.Instance.ShowFinalPanel(true);
            AudioManager.Instance.StopMainSound();
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.GameOver);
        }

        #endregion
    }
}