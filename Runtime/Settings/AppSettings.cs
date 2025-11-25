using System;
using Ouroboros.Common.Audio;
using Ouroboros.Common.Exceptions;
using Ouroboros.Common.Logging;
using Ouroboros.Common.Services;
using Ouroboros.Common.Utils;
using UnityEngine;

namespace Ouroboros.Common.Settings
{
    public class AppSettings : MonoBehaviour
    {
        public RegisteredCallback OnInit { get; } = new RegisteredCallback(); 

        private SettingsManager settingsManager;

        private const string SFXKey = "sfx";
        private const string MusicKey = "music";

        private void Awake()
        {
            ServiceLocator.Register(this);
            settingsManager = GetComponent<SettingsManager>();
        }

        public async void Start()
        {
            await settingsManager.InitTask;

            try
            {
                UpdateSFX();
                UpdateMusic();

                OnInit?.Invoke();
            }
            catch (Exception ex)
            {
                Logs.Error<SettingsManager>(ex.PrintException());
            } 
        }

        private void UpdateMusic()
        {
            AudioManager.SetMusicVolume(GetMusicVolume());
        }

        private void UpdateSFX()
        {
            AudioManager.SetSFXVolume(GetSFXVolume());
        }

        public bool IsSFXOn()
        {
            return settingsManager.GetSettingsBool(SFXKey);
        }

        public void SetSFXOn(bool isOn)
        {
            settingsManager.SetSettings(SFXKey, isOn);
            UpdateSFX();
        }

        public bool IsMusicOn()
        {
            return GetMusicVolume() > 0f;
        }

        public float GetMusicVolume()
        {
            return settingsManager.GetSettingsFloat(MusicKey);
        }

        public float GetSFXVolume()
        {
            return settingsManager.GetSettingsFloat(SFXKey);
        }

        public void SetMusicVolume(float volume)
        {
            settingsManager.SetSettings(MusicKey, volume);
            UpdateMusic();
        }

        public void SetSFXVolume(float volume)
        {
            settingsManager.SetSettings(SFXKey, volume);
            UpdateSFX();
        }
    }
}