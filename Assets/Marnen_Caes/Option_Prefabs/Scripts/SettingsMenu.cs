using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;


namespace MC
{

    public class SettingsMenu : MonoBehaviour
    {

        [Serializable]
        public struct References
        {
            [Header("Video")]
            public Toggle fullscreenToggle;

            //[Header("Audio")]
            //public Toggle muteVolumeToggle;
            //public Slider globalVolumeSlider;

        }
        [SerializeField]
        private References references = new References();

        Resolution[] resolutions;
        [Space(16)]
        public TMP_Dropdown resolutionDropdown;
        //public AudioMixer audioMixer;
#if UNITY_STANDALONE
        private void Start()
        {


            resolutions = Screen.resolutions;
            resolutions = resolutions.Reverse().ToArray();
            resolutionDropdown.ClearOptions();

            List<string> options = new List<string>();
            int currentResolutionIndex = 0;
            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);
                if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }

            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();

            //Init
            SetFullscreen(Settings.Fullscreen);
            references.fullscreenToggle.isOn = Settings.Fullscreen;
            SetResolution(Settings.Resolution);

            SetQuality(Settings.Quality);



            ////SetMute(Settings.Mute);
            //references.muteVolumeToggle.isOn = Settings.Mute;

            ////SetGlobalVolume(Settings.GlobalVolume);
            //references.globalVolumeSlider.value = Settings.GlobalVolume;
        }
        #region Video     

        public void SetFullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
            Settings.Fullscreen = isFullscreen;
        }
        public void SetResolution(int resolutionIndex)
        {
            if (resolutions.Length > resolutionIndex)
            {
                Resolution resolution = resolutions[resolutionIndex];
                Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            }
            else
            {
                resolutionIndex = 0;
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, Screen.fullScreen);
            }
            Settings.Resolution = resolutionIndex;
        }

        public void SetQuality(int qualityIndex)
        {
            if (QualitySettings.names.Length <= qualityIndex)
            {
                qualityIndex = QualitySettings.GetQualityLevel();
            }

            QualitySettings.SetQualityLevel(qualityIndex);
            Settings.Quality = qualityIndex;
        }

        #endregion
        #region Audio
        //public void SetMute(bool isMuted)
        //{
        //    audioMixer.SetFloat(Settings.MUTE, Mathf.Log10(isMuted == true ? 1 : 0.0001f) * 20);
        //    Settings.Mute = isMuted;
        //}
        //public void SetGlobalVolume(float volume)
        //{
        //    audioMixer.SetFloat(Settings.GLOBALVOLUME, Mathf.Log10(volume) * 20);
        //    Settings.GlobalVolume = volume;
        //}
        #endregion
#endif
    }


}

