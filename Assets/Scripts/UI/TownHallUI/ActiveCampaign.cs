using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Assets.Scripts.Buildings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace Assets.Scripts.UI.TownHallUI
{
    public class ActiveCampaign : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private RectTransform image;
        private TextMeshProUGUI text;
        private RawImage textBackgroundImage;
        private TextMeshProUGUI tooltipText;

        public SensibilizationCampaign Campaign;
        [SerializeField] private int campaignIndex;
        [SerializeField] SensibilizationCampaign.SensibilizationCampaignOptions CampaignOption;

        private bool inside;
        private float timeInside;

        private void Awake()
        {
            image = transform.GetChild(0).GetComponent<RectTransform>();
            textBackgroundImage = transform.GetChild(1).GetComponent<RawImage>();
            text = textBackgroundImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            tooltipText = transform.GetChild(2).GetComponent<TextMeshProUGUI>();

            TownHall.Instance.RegisterCampaign(campaignIndex, this);
            inside = false;
            timeInside = 0.0f;
        }

        private void OnEnable()
        {
            textBackgroundImage.color = new Color(1, 1, 1, 0.25f);
        }

        private void Start()
        {
            textBackgroundImage.color = new Color(1, 1, 1, 0f);
        }


        private void Update()
        {
            if(inside)
            {
                timeInside += Time.unscaledDeltaTime;
                if(timeInside >= 0.2f)
                {
                    tooltipText.gameObject.SetActive(true);
                    tooltipText.text = Campaign.CampaignName + " Active";
                }
                image.localPosition = new Vector3(-40 + TownHall.Instance.CampaignsCompletionPercentage[Campaign] * 40, 0, 0);
                image.sizeDelta = new Vector2(80f * TownHall.Instance.CampaignsCompletionPercentage[Campaign], 80);
            }

            if(TownHall.Instance.CampaignsCompletionPercentage != null && Campaign != null && TownHall.Instance.CampaignsCompletionPercentage[Campaign] >= 1)
            {
                image.sizeDelta = new Vector2(0, 80);

                gameObject.SetActive(false);
            }
        }
        public void RegisterCampaign(SensibilizationCampaign sensCampaign)
        {
            Campaign = sensCampaign;
            text.color = Color.black;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            timeInside = 0.0f;

            if (Campaign != null)
                inside = true;
        }



        public void OnPointerExit(PointerEventData eventData)
        {
            inside = false;
            image.sizeDelta = new Vector2(0, 80);
            tooltipText.gameObject.SetActive(false);
        }

    }
}
