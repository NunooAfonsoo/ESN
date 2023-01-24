using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Assets.Scripts.Buildings;


namespace Assets.Scripts.UI.TownHallUI
{
    public class ActiveLaw : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private RawImage image;
        private RawImage textBackgroundImage;
        private TextMeshProUGUI text;
        private TextMeshProUGUI tooltipText;

        private bool inside;
        private float timeInside;

        public Law Law;


        private void Awake()
        {
            image = transform.GetChild(0).GetComponent<RawImage>();
            textBackgroundImage = transform.GetChild(1).GetComponent<RawImage>();
            text = textBackgroundImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            tooltipText = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            TownHall.Instance.RegisterLaw(this);
            timeInside = 0;
        }

        private void OnEnable()
        {
            textBackgroundImage.color = new Color(1, 1, 1, 0.25f);
        }

        private void Update()
        {
            if (inside)
            {
                timeInside += Time.unscaledDeltaTime;
                if(timeInside >= 0.2f)
                {
                    tooltipText.gameObject.SetActive(true);
                    tooltipText.text = Law.LawName + " Active";
                }
                image.color = Color.white;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            timeInside = 0.0f;
            inside = true;
        }


        public void ChangeText()
        {
            switch (Law.LawOption)
            {
                case Law.LawOptions.Lockdown:
                    text.text = "I";
                    break;
                case Law.LawOptions.RemoteWork:
                    text.text = "II";
                    break;
                case Law.LawOptions.ForcedLabour:
                    text.text = "III";
                    break;
            }
            text.color = new Color(0, 0, 0, 1);
        }


        public void RegisterLaw(Law law)
        {
            Law = law;
            text.color = new Color(0, 0, 0, 0);
        }


        private void OnDisable()
        {
            Law = null;
        }


        public void OnPointerExit(PointerEventData eventData)
        {
            inside = false;
            image.color = new Color(1, 1, 1, 0f);
            tooltipText.gameObject.SetActive(false);
        }
    }
}