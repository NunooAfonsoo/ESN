using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Constants;


namespace Assets.Scripts.Managers {
    public class PauseMenuManager : MonoBehaviour
    {
        public void ResumeGame()
        {
            Time.timeScale = 1;
            SceneManager.UnloadSceneAsync(Scenes.PauseMenu);
        }


        public void MainMenu()
        {
            SceneManager.LoadScene(Scenes.MainMenu);
        }


        public void QuitGame()
        {
            Application.Quit();
        }
    }
}