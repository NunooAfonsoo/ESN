using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Assets.Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        private void Awake()
        {
            Instance = this;
            if(!SceneManager.GetSceneByName("UI").isLoaded)
            {
                SceneManager.LoadSceneAsync("UI", LoadSceneMode.Additive);
            }
            else
            {
                SceneManager.UnloadSceneAsync("UI");
            }
        }


        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                Time.timeScale = 1;
            }
            else if(Input.GetKeyDown(KeyCode.Alpha2))
            {
                Time.timeScale = 2;

            }
            else if(Input.GetKeyDown(KeyCode.Alpha3))
            {
                Time.timeScale = 4;
            }
            else if(Input.GetKeyDown(KeyCode.Alpha4))
            {
                Time.timeScale = 16;
            }
        }
    }
}
