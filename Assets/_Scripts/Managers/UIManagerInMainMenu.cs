using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Managers
{
    public class UIManagerInMainMenu : MonoBehaviour
    {
        #region Inspector Variables

        [Header("Start Menu")] [SerializeField] private Button startButton;

        [Header("Transition")] [SerializeField]
        private Image blackScreen;

        #endregion

        #region Public Variables

        #endregion

        #region Private Variables

        #endregion

        #region Unity Methods

        private void Start()
        {
            startButton.onClick.AddListener(StartGame);
        }

        #endregion

        #region Public Methods

        public void StartGame()
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.buttonClick);
            blackScreen.DOFade(1f, 1.5f).SetEase(Ease.OutSine)
                .OnPlay(() => GameManager.Instance.ChangeState(GameState.Starting))
                .OnComplete(() => { MySceneManager.LoadScene(MySceneManager.Scenes.GameScene); })
                .Play();
        }

        #endregion

        #region Private Methods

        #endregion
    }
}