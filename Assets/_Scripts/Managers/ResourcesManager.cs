using Utilities;
using UnityEngine;

namespace Managers
{
    public class ResourcesManager : MonoBehaviourSinglenton<ResourcesManager>
    {
        #region Inspector Variables

        [SerializeField] private int StartGold;

        #endregion

        #region Public Variables

        public int CurrentGold => _currentGold;

        #endregion

        #region Private Variables

        private int _currentHabitantes;
        private int _currentGastosPorSegundo;
        private float _currentGoldPorSegundo;
        private int _currentGold;

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
            AddGold(StartGold);
            AddHabitante(0);
            AddGastos(0);
            AddGoldPorSegundo(0);
        }

        private void Update()
        {
            //Cada segundo se restan los gatos
            if (Time.frameCount % Application.targetFrameRate == 0 && GameManager.CurrentGameState == GameState.Playing)
            {
                AddGold(-_currentGastosPorSegundo);
            }

            if (_currentGold < -500 && GameManager.CurrentGameState == GameState.Playing)
            {
                GameOver();
            }
        }

        #endregion

        #region Public Methods

        public void AddHabitante(int h)
        {
            _currentHabitantes += h;
            UIManagerInGame.Instance.UpdateInhabitantsNumberTMP(_currentHabitantes);
        }

        public void AddGold(int g)
        {
            _currentGold += g;
            UIManagerInGame.Instance.UpdateGold(_currentGold);
        }

        public void AddGastos(int g)
        {
            _currentGastosPorSegundo += g;
            UIManagerInGame.Instance.UpdateGastos(_currentGastosPorSegundo);
        }

        public void AddGoldPorSegundo(float g)
        {
            _currentGoldPorSegundo += g;
            UIManagerInGame.Instance.UpdateGoldPorSegundo((int)_currentGoldPorSegundo);
        }

        public void RestartAllInfo()
        {
            AddGold(-_currentGold);
            AddGold(StartGold);
            AddHabitante(-_currentHabitantes);
            AddGoldPorSegundo(-_currentGoldPorSegundo);
            AddGastos(-_currentGastosPorSegundo);
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