using System;
using Proyecto.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Proyecto.Managers
{
    public class AudioManager : Singlenton<AudioManager>
    {
        #region Inspector Variables

        [SerializeField] private AudioSource MainAudioSource;
        [SerializeField] private AudioSource SFXAudioSource;
        [SerializeField] private AudioSource SFX_ClickAudioSource;
        
        [Tooltip("Orden: construccion, habitante, destroy, clickOnMap, nivelUp, buttonClick, GameOver")]
        [SerializeField] private AudioClip[] SFX_AudioClips;

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
            MainAudioSource.Play();
        }
        
        public void StopMainSound()
        {
            MainAudioSource.Stop();
        }

        public void PlaySFXSound(SFX_Type type)
        {
            switch (type)
            {
                case SFX_Type.buildBuilding:
                    SFXAudioSource.pitch = Random.Range(1,1.6f);
                    PlaySFXSound(SFX_AudioClips[0], SFXAudioSource);
                    break;
                case SFX_Type.newHabitante:
                    SFXAudioSource.pitch = Random.Range(1,1.6f);
                    PlaySFXSound(SFX_AudioClips[1], SFXAudioSource);
                    break;
                case SFX_Type.detroyBuilding:
                    SFXAudioSource.pitch = Random.Range(1,1.6f);
                    PlaySFXSound(SFX_AudioClips[2], SFXAudioSource);
                    break;
                case SFX_Type.clickOnMap:
                    SFXAudioSource.pitch = Random.Range(1,1.6f);
                    PlaySFXSound(SFX_AudioClips[3], SFX_ClickAudioSource);
                    break;
                case SFX_Type.levelUp:
                    SFXAudioSource.pitch = Random.Range(1,1.6f);
                    PlaySFXSound(SFX_AudioClips[4], SFXAudioSource);
                    break;
                case SFX_Type.buttonClick:
                    SFXAudioSource.pitch = 1;
                    PlaySFXSound(SFX_AudioClips[5], SFX_ClickAudioSource);
                    break;
                case SFX_Type.GameOver:
                    SFXAudioSource.pitch = 1;
                    PlaySFXSound(SFX_AudioClips[6], SFXAudioSource);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
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