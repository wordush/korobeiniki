using System;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class SettingInterface : MonoBehaviour
    {
        public Slider musicVolume;
        public Slider perfVolume;

        public Text resolution;
        private Resolutions _reso;

        private void Start()
        {
        
            if (!PlayerPrefs.HasKey("MusicVolume"))
            {
                PlayerPrefs.SetFloat("MusicVolume",musicVolume.value);
                PlayerPrefs.SetFloat("PrefVolume", perfVolume.value);
                PlayerPrefs.SetInt("Resolution", (int)_reso);
            }
            else
            {
                musicVolume.value = PlayerPrefs.GetFloat("MusicVolume");
                perfVolume.value = PlayerPrefs.GetFloat("PrefVolume");
                _reso = (Resolutions)PlayerPrefs.GetInt("Resolution");
                SetResText();
            }
        }

        public void SavePrefs()
        {
            PlayerPrefs.SetFloat("MusicVolume", musicVolume.value);
            PlayerPrefs.SetFloat("PrefVolume", perfVolume.value);
            PlayerPrefs.SetInt("Resolution", (int)_reso);
            PlayerPrefs.Save();
        }

        private enum Resolutions
        {
            Res1920X1080,
            Res1366X768

        }

        public void ResUp()
        {
            if ((int)_reso > 0)
            {
                _reso -= 1;
                SetResText();
            }
        }

        public void ResDown()
        {
            if ((int) _reso >= 1) return;
            _reso += 1;
            SetResText();
        }

        private void SetResText()
        {
            switch (_reso)
            {
                case Resolutions.Res1920X1080:
                    resolution.text = "1920x1080";
                    break;
                case Resolutions.Res1366X768:
                    resolution.text = "1366x768";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Update()
        {
            SetMusicVolume();
            SetSetPerfVolume();
        }


        private void SetMusicVolume()
        {
            GameObject.FindGameObjectWithTag("Music").GetComponent<MusicSource>().SetMusicVolume(musicVolume.value * 0.01f);
        }

        private void SetSetPerfVolume()
        {
            GameObject.FindGameObjectWithTag("Music").GetComponent<MusicSource>().SetPerfVolume(perfVolume.value * 0.01f);
        }
    }
}
