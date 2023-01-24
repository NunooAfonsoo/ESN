using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Constants;

namespace Assets.Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public bool PauseMenuActive
        {
            get
            {
                return SceneManager.GetSceneByName("PauseMenu").isLoaded;
            }
        }


        private void Awake()
        {
            Instance = this;
            if(!SceneManager.GetSceneByName(Scenes.UI).isLoaded)
            {
                SceneManager.LoadSceneAsync(Scenes.UI, LoadSceneMode.Additive);
                SceneManager.LoadScene(Scenes.GameWon, LoadSceneMode.Additive);
            }
            else
            {
                SceneManager.UnloadSceneAsync(Scenes.UI);
            }
        }


        private void Update()
        {
            if(!PauseMenuActive)
            { 
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    Time.timeScale = 1;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    Time.timeScale = 2;

                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    Time.timeScale = 4;
                }
            }
            if(Input.GetKeyDown(KeyCode.Escape) && UIManager.Instance.ActiveMenu == null)
            {
                if (!SceneManager.GetSceneByName(Scenes.PauseMenu).isLoaded)
                {
                    Time.timeScale = 0;
                    SceneManager.LoadSceneAsync(Scenes.PauseMenu, LoadSceneMode.Additive);
                }
                else
                {
                    Time.timeScale = 1;
                    SceneManager.UnloadSceneAsync(Scenes.PauseMenu);
                }
            }
        }
    }
}
