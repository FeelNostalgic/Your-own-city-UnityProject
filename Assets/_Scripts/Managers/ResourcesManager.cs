using System;
using Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Managers
{
    public class ResourcesManager : MonoBehaviourSinglenton<ResourcesManager>
    {
        #region Inspector Variables

        [FormerlySerializedAs("StartGold")] [SerializeField] private int startGold;

        #endregion

        #region Public Variables

        public int CurrentGold => _currentGold;

        public event Action<int> OnGoldUpdate;
        public event Action<int> OnResidentUpdate;
        public event Action<int> OnCostsPerSecondUpdate;
        public event Action<int> OnGoldPerSecondUpdate;

        #endregion

        #region Private Variables

        private int _currentInhabitants;
        private int _currentCostsPerSecond;
        private float _currentGoldPerSecond;
        private int _currentGold;

        public ResourcesManager(int startGold)
        {
            this.startGold = startGold;
        }

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

            if (_currentGold < -500 && GameManager.CurrentGameState == GameState.Playing)
            {
                GameOver();
            }
        }

        #endregion

        #region Public Methods

        public void AddResident(int h)
        {
            _currentInhabitants += h;
            OnResidentUpdate?.Invoke(_currentInhabitants);
        }

        public void AddGold(int g)
        {
            _currentGold += g;
            OnGoldUpdate?.Invoke(_currentGold);
        }

        public void AddCosts(int g)
        {
            _currentCostsPerSecond += g;
            OnCostsPerSecondUpdate?.Invoke(_currentCostsPerSecond);
        }

        public void AddGoldPerSecond(float g)
        {
            _currentGoldPerSecond += g;
            OnGoldPerSecondUpdate?.Invoke((int)_currentGoldPerSecond);
        }

        public void RestartAllInfo()
        {
            AddGold(-_currentGold);
            AddGold(startGold);
            AddResident(-_currentInhabitants);
            AddGoldPerSecond(-_currentGoldPerSecond);
            AddCosts(-_currentCostsPerSecond);
        }

        #endregion

        #region Private Methods

        private void GameOver()
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