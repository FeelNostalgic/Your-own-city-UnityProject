using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Managers
{
    public class UIManagerInMainMenu : MonoBehaviour
    {
        #region Inspector Variables

        [Header("Start Panel")] 
        [SerializeField] private GameObject startPanel;
        [SerializeField] private Button startButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button controlsButton;
        [SerializeField] private Button objectivesButton;
        
        [Header("Options Panel")] 
        [SerializeField] private GameObject optionsPanel;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider effectsVolumeSlider;
        [SerializeField] private TMP_Dropdown languageDropdown;
        [SerializeField] private Button optionsBackButton;
        
        [Header("Controls Panel")]
        [SerializeField] private GameObject controlsPanel;
        [SerializeField] private Button controlsBackButton;
        
        [Header("Objectives Panel")]
        [SerializeField] private GameObject objectivesPanel;
        [SerializeField] private Button objectivesBackButton;
        
        [Header("Transition")] [SerializeField]
        private Image blackScreen;

        #endregion

        #region Public Variables

        public enum UIPanels
        {
            none, startMenu, optionsMenu, controlsMenu, objectivesMenu
        }
        
        #endregion

        #region Private Variables

        private UIPanels _currentPanel = UIPanels.startMenu;
        private UIPanels _lastPanel;
        private GameObject _currentActivePanel;
        
        #endregion

        #region Unity Methods

        private void Start()
        {
            startPanel.SetActive(false);
            optionsPanel.SetActive(false);
            BuildOptionsListeners();
            controlsPanel.SetActive(false);
            objectivesPanel.SetActive(false);
            BuildListeners();
            _currentActivePanel = optionsPanel;
            ChangeUIPanel(UIPanels.startMenu);
        }
        
        #endregion

        #region Public Methods
        
//
        
        #endregion

        #region Private Methods

        private void BuildOptionsListeners()
        {
            StartCoroutine(ILanguageSelector());
            musicVolumeSlider.value = 1;
            musicVolumeSlider.onValueChanged.AddListener(AudioManager.Instance.ManageMusicVolume);
            effectsVolumeSlider.value = 1;
            effectsVolumeSlider.onValueChanged.AddListener(AudioManager.Instance.ManageSFXVolume);
        }

        private IEnumerator ILanguageSelector()
        {
            // Wait for the localization system to initialize
            yield return LocalizationSettings.InitializationOperation;

            // Generate list of available Locales
            var options = LocalizationSettings.AvailableLocales.Locales.Select(locale => new TMP_Dropdown.OptionData(locale.name)).ToList();
            languageDropdown.options = options;

            languageDropdown.value = 0;
            languageDropdown.onValueChanged.AddListener(LocaleSelected);
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
        }

        private static void LocaleSelected(int index)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        }
        
        private void BuildListeners()
        {
            startButton.onClick.AddListener(()=>
            {
                PlayClickSound();
                StartGame();
            });
            optionsButton.onClick.AddListener(()=>
            {
                PlayClickSound();
                ChangeUIPanel(UIPanels.optionsMenu);
            });
            controlsButton.onClick.AddListener(() =>
            {
                PlayClickSound();
                ChangeUIPanel(UIPanels.controlsMenu);
            });
            objectivesButton.onClick.AddListener(() =>
            {
                PlayClickSound();
                ChangeUIPanel(UIPanels.objectivesMenu);
            });
            controlsBackButton.onClick.AddListener(BackPanel);
            optionsBackButton.onClick.AddListener(BackPanel);
            objectivesBackButton.onClick.AddListener(BackPanel);
        }
        
        private void StartGame()
        {
            PlayClickSound();
            blackScreen.DOFade(1f, 1.5f).SetEase(Ease.OutSine)
                .OnPlay(() => GameManager.Instance.ChangeState(GameState.Starting))
                .OnComplete(() => { MySceneManager.LoadScene(MySceneManager.Scenes.GameScene); })
                .Play();
        }
        
        private void BackPanel()
        {
            PlayClickSound();
            ChangeUIPanel(_lastPanel);
        }

        private static void PlayClickSound()
        {
            AudioManager.Instance.PlaySFXSound(AudioManager.SFX_Type.buttonClick);
        }
        
        private void ChangeUIPanel(UIPanels newPanel)
        {
            _lastPanel = _currentPanel;
            _currentPanel = newPanel;
            _currentActivePanel!.SetActive(false);
            switch (_currentPanel)
            {
                case UIPanels.startMenu:
                    startPanel.SetActive(true);
                    _currentActivePanel = startPanel;
                    break;
                case UIPanels.optionsMenu:
                    optionsPanel.SetActive(true);
                    _currentActivePanel = optionsPanel;
                    break;
                case UIPanels.controlsMenu:
                    controlsPanel.SetActive(true);
                    _currentActivePanel = controlsPanel;
                    break;
                case UIPanels.objectivesMenu:
                    objectivesPanel.SetActive(true);
                    _currentActivePanel = objectivesPanel;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        #endregion
    }
}