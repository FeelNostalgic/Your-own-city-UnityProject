using System;
using Utilities;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Managers
{
    public class AudioManager : MonoBehaviourSinglenton<AudioManager>
    {
        #region Inspector Variables

        [SerializeField] private AudioMixer audioMixer;
        
        [FormerlySerializedAs("MainAudioSource")] [SerializeField] private AudioSource mainAudioSource;
        [FormerlySerializedAs("SFXAudioSource")] [SerializeField] private AudioSource sfxAudioSource;
        [FormerlySerializedAs("SFX_ClickAudioSource")] [SerializeField] private AudioSource sfxClickAudioSource;
        
        [FormerlySerializedAs("SFX_AudioClips")]
        [Tooltip("Orden: construccion, habitante, destroy, clickOnMap, nivelUp, buttonClick, GameOver")]
        [SerializeField] private AudioClip[] sfxAudioClips;

        #endregion

        #region Public Variables

        public enum SFX_Type
        {
            buildBuilding, newHabitante, detroyBuilding, clickOnMap, levelUp, buttonClick, GameOver
        }
        #endregion

        #region Private Variables

        #endregion

        #region Unity Methods
        
        #endregion

        #region Public Methods

        public void PlayMainSound()
        {
            mainAudioSource.Play();
        }
        
        public void StopMainSound()
        {
            mainAudioSource.Stop();
        }

        public void PlaySFXSound(SFX_Type type)
        {
            switch (type)
            {
                case SFX_Type.buildBuilding:
                    sfxAudioSource.pitch = Random.Range(1,1.6f);
                    PlaySFXSound(sfxAudioClips[0], sfxAudioSource);
                    break;
                case SFX_Type.newHabitante:
                    sfxAudioSource.pitch = Random.Range(1,1.6f);
                    PlaySFXSound(sfxAudioClips[1], sfxAudioSource);
                    break;
                case SFX_Type.detroyBuilding:
                    sfxAudioSource.pitch = Random.Range(1,1.6f);
                    PlaySFXSound(sfxAudioClips[2], sfxAudioSource);
                    break;
                case SFX_Type.clickOnMap:
                    sfxAudioSource.pitch = Random.Range(1,1.6f);
                    PlaySFXSound(sfxAudioClips[3], sfxClickAudioSource);
                    break;
                case SFX_Type.levelUp:
                    sfxAudioSource.pitch = Random.Range(1,1.6f);
                    PlaySFXSound(sfxAudioClips[4], sfxAudioSource);
                    break;
                case SFX_Type.buttonClick:
                    sfxAudioSource.pitch = Random.Range(0.9f,1.1f);
                    PlaySFXSound(sfxAudioClips[5], sfxClickAudioSource);
                    break;
                case SFX_Type.GameOver:
                    sfxAudioSource.pitch = 1;
                    PlaySFXSound(sfxAudioClips[6], sfxAudioSource);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public void ManageMusicVolume(float sliderValue)
        {
            var volume = sliderValue > 0 ? Mathf.Log10(sliderValue) * 20 : -80f;
            audioMixer.SetFloat("musicVol", volume);
        }

        public void ManageSFXVolume(float sliderValue)
        {
            var volume = sliderValue > 0 ? Mathf.Log10(sliderValue) * 20 : -80f;
            audioMixer.SetFloat("sfxVol", volume);
        }
        
        #endregion

        #region Private Methods
        
        private void PlaySFXSound(AudioClip audio, AudioSource audioSource)
        {
            audioSource.PlayOneShot(audio);
        }
        
        #endregion
    }
}