using System.Collections;
using System.Linq;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace UI
{ 
	public class OptionsPanelController : MonoBehaviour
	{
		#region Inspector Variables
		
		[SerializeField] private Slider musicVolumeSlider;
		[SerializeField] private Slider effectsVolumeSlider;
		[SerializeField] private TMP_Dropdown languageDropdown;
		[SerializeField] private Button optionsBackButton;
		[SerializeField] private UnityEvent backEvent;
		
		
		#endregion
	
		#region Public Variables
		//
		#endregion

		#region Private Variables
		//
		#endregion

		#region Unity Methods

		private void Start()
		{
			BuildOptionsListeners();
		}

		#endregion

		#region Public Methods
		//
		#endregion

		#region Private Methods
		
		private void BuildOptionsListeners()
		{
			StartCoroutine(ILanguageSelector());
			musicVolumeSlider.value = GameManager.MusicVolume;
			musicVolumeSlider.onValueChanged.AddListener(AudioManager.Instance.ManageMusicVolume);
			effectsVolumeSlider.value = GameManager.EffectsVolume;
			effectsVolumeSlider.onValueChanged.AddListener(AudioManager.Instance.ManageSFXVolume);
			optionsBackButton.onClick.AddListener(backEvent.Invoke);
		}
		
		private IEnumerator ILanguageSelector()
		{
			// Wait for the localization system to initialize
			yield return LocalizationSettings.InitializationOperation;

			// Generate list of available Locales
			var options = LocalizationSettings.AvailableLocales.Locales.Select(locale => new TMP_Dropdown.OptionData(locale.name)).ToList();
			languageDropdown.options = options;

			languageDropdown.value = GameManager.SelectedLocale;
			languageDropdown.onValueChanged.AddListener(LocaleSelected);
			LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
		}
		
		private static void LocaleSelected(int index)
		{
			LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
			GameManager.SelectedLocale = index;
		}
		
		#endregion
	}
}