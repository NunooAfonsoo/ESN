using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


namespace Assets.Scripts.UI.EndGame
{
    public class GameWonUIManager : MonoBehaviour
    {
        public static GameWonUIManager Instance;
        public TextMeshProUGUI VictoryText;
        public RawImage BackgroundImage;


        void Awake()
        {
            Instance = this;
            gameObject.SetActive(false);

            Debug.Log("AWAKEE");
        }


    }
}
