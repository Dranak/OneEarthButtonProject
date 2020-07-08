using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MC
{
    public class Settings
    {
        //These keys are used in PlayerPref & Mixer exposed parameter.
        #region PlayerPrefsKeys
        public const string FULLSCREEN = "fullscreen";
        public const string RESOLUTION = "resolution";
        public const string QUALITY = "quality";

        public const string GLOBALVOLUME = "globalVolume";
        public const string MUTE = "mute";
        #endregion
        /// <summary>
        /// Use static settings to change value in playerPrefs
        /// </summary>
        #region Static_Settings
        //VIDEO
        public static bool Fullscreen
        {
            get => PlayerPrefs.GetInt(FULLSCREEN, 1) == 1 ? true : false;
            set => PlayerPrefs.SetInt(FULLSCREEN, value == true ? 1 : 0);
        }
        public static int Resolution
        {
            get => PlayerPrefs.GetInt(RESOLUTION, 0);
            set => PlayerPrefs.SetInt(RESOLUTION, value);
        }
        public static int Quality
        {
            get => PlayerPrefs.GetInt(QUALITY, 0);
            set => PlayerPrefs.SetInt(QUALITY, value);
        }
        //AUDIO
        public static bool Mute
        {
            get => PlayerPrefs.GetFloat(MUTE, 0.0001f) == 1 ? true : false;
            set => PlayerPrefs.SetFloat(MUTE, value == true ? 1 : 0.0001f);
        }
        public static float GlobalVolume
        {
            get =>  PlayerPrefs.GetFloat(GLOBALVOLUME, 1);
            set => PlayerPrefs.SetFloat(GLOBALVOLUME, value);
        }
        #endregion
    }
}

