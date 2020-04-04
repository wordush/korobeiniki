using UnityEngine;
using UnityEngine.Serialization;

namespace Menu
{
    public class Menuhandler : MonoBehaviour
    {
        [FormerlySerializedAs("MainMenu")] public Canvas mainMenu;
        [FormerlySerializedAs("SinglePlayerMenu")] public Canvas singlePlayerMenu;
        [FormerlySerializedAs("MultiPlayerMenu")] public Canvas multiPlayerMenu;
        [FormerlySerializedAs("SettingsMenu")] public Canvas settingsMenu;

        private void Start()
        {
            mainMenu.gameObject.SetActive(true);
            singlePlayerMenu.gameObject.SetActive(false);
            multiPlayerMenu.gameObject.SetActive(false);
            settingsMenu.gameObject.SetActive(false);
        }

        public void SceneLoad(int id)
        {
            GameObject.FindGameObjectWithTag("SceneHandler").GetComponent<SceneHandler>().NiceSceneLoader(id);
        }

        public void Exit()
        {
            Application.Quit();
        }
    
        public void MainMenuSwitch()
        {
            mainMenu.gameObject.SetActive(true);
            singlePlayerMenu.gameObject.SetActive(false);
            multiPlayerMenu.gameObject.SetActive(false);
            settingsMenu.gameObject.SetActive(false);
        }

    


        public void Singlplayer()
        {
            mainMenu.gameObject.SetActive(false);
            singlePlayerMenu.gameObject.SetActive(true);
        }

        public void Multiplayer()
        {
            mainMenu.gameObject.SetActive(false);
            multiPlayerMenu.gameObject.SetActive(true);
        }

        public void Options()
        {
            mainMenu.gameObject.SetActive(false);
            settingsMenu.gameObject.SetActive(true);
        }
    }
}
