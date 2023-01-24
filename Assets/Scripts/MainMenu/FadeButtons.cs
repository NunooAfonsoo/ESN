using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using TMPro;


namespace MainMenu
{
    public class FadeButtons : MonoBehaviour, IPointerEnterHandler
    {
        [SerializeField] private Image playButtonImg;
        [SerializeField] private TextMeshProUGUI playButtonTxt;
        [SerializeField] private Image exitButtonImg;
        [SerializeField] private TextMeshProUGUI exitButtonTxt;

        private float fadeState = 0;
        [SerializeField] private float fadeStep;


        public void OnPointerEnter(PointerEventData eventData)
        {
            StopAllCoroutines();
            StartCoroutine(ShowButtons());
        }


        private IEnumerator ShowButtons()
        {
            while (fadeState <= 1)
            {
                fadeState += fadeStep;

                playButtonImg.color = exitButtonImg.color = new Color(1, 1, 1, fadeState);
                playButtonTxt.color = new Color(playButtonTxt.color.r, playButtonTxt.color.g, playButtonTxt.color.b, fadeState);
                exitButtonTxt.color = new Color(exitButtonTxt.color.r, exitButtonTxt.color.g, exitButtonTxt.color.b, fadeState);

                yield return null;
            }
        }


        private IEnumerator HideButtons()
        {
            while (fadeState >= 0)
            {
                fadeState -= fadeStep / 2;

                playButtonImg.color = exitButtonImg.color = new Color(1, 1, 1, fadeState);
                playButtonTxt.color = new Color(playButtonTxt.color.r, playButtonTxt.color.g, playButtonTxt.color.b, fadeState);
                exitButtonTxt.color = new Color(exitButtonTxt.color.r, exitButtonTxt.color.g, exitButtonTxt.color.b, fadeState);

                yield return null;
            }
        }
    }
}
