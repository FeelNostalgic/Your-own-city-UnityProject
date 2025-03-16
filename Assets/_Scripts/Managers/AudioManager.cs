using System;
using Utilities;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace Managers
{
    public class AudioManager : MonoBehaviourSinglenton<AudioManager>
    {
        #region Inspector Variables

        [SerializeField] private AudioMixer audioMixer;
        
        [SerializeField] private AudioSource mainAudioSource;
        [SerializeField] private AudioSource sfxAudioSource;
        [SerializeField] private AudioSource sfxClickAudioSource;
        
        [Tooltip("Orden: building, resident, destroy, clickOnMap, levelUp, buttonClick, GameOver")]
        [SerializeField] private AudioClip[] sfxAudioClips;

        #endregion

        #region Public Variables

        public enum SFX_Type
        {
            buildBuilding, newResident, detroyBuilding, clickOnMap, levelUp, buttonClick, GameOver
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
                case SFX_Type.newResident:
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
            GameManager.MusicVolume = volume;
            audioMixer.SetFloat("musicVol", volume);
        }

        public void ManageSFXVolume(float sliderValue)
        {
            var volume = sliderValue > 0 ? Mathf.Log10(sliderValue) * 20 : -80f;
            GameManager.EffectsVolume = volume;
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