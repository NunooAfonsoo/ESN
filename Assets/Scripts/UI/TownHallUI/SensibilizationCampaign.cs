using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Managers;
using Assets.Scripts.Citizens;
using Assets.Scripts.Clock;
using Assets.Scripts.Buildings;
using TMPro;

namespace Assets.Scripts.UI.TownHallUI
{
    public class SensibilizationCampaign : MonoBehaviour
    {
        [SerializeField] private int campaignCost;

        [SerializeField] private Button doCampaignButton;
        private Button campaignButton;

        private GameObject activeCampaignGO;
        [SerializeField] private GameObject campaignDescription;
        public string CampaignName;

        [SerializeField] private int maxTimesDone;

        [SerializeField] private float campaignDuration;
        public enum SensibilizationCampaignOptions
        {
            NewDisease,
            MaskUse,
            HandDisinfection,
            SonnerTreatment,
            ApprovalRatingBoost
        }

        public SensibilizationCampaignOptions SensCampaign;



        private void Awake()
        {
            campaignButton = transform.GetChild(0).GetComponent<Button>();
            campaignButton.onClick.AddListener(delegate { OpenDescription(); });

            TownHallUIManager.Instance.RegisterCampaign(this);
            TownHall.Instance.CampaignsCompletionPercentage.Add(this, 0);
        }
        private void Start()
        {
            activeCampaignGO = null;
            switch (SensCampaign)
            {
                case SensibilizationCampaignOptions.NewDisease:
                    activeCampaignGO = GameObject.Find("Sens1Active");
                    GameObject.Find("Sens1Active").GetComponent<ActiveCampaign>().RegisterCampaign(this);
                    break;
                case SensibilizationCampaignOptions.MaskUse:
                    activeCampaignGO = GameObject.Find("Sens2Active");
                    GameObject.Find("Sens2Active").GetComponent<ActiveCampaign>().RegisterCampaign(this);
                    break;
                case SensibilizationCampaignOptions.HandDisinfection:
                    activeCampaignGO = GameObject.Find("Sens3Active");
                    GameObject.Find("Sens3Active").GetComponent<ActiveCampaign>().RegisterCampaign(this);
                    break;
                case SensibilizationCampaignOptions.ApprovalRatingBoost:
                    activeCampaignGO = GameObject.Find("Sens4Active");
                    GameObject.Find("Sens4Active").GetComponent<ActiveCampaign>().RegisterCampaign(this);
                    break;
            }
            activeCampaignGO.GetComponent<RectTransform>().sizeDelta = new Vector2(80, 80);
            activeCampaignGO.SetActive(false);
        }

        private void Update()
        {
            CheckNodeRequirements();
        }


        private void CheckNodeRequirements()
        {
            if (EconomyManager.Instance.UpdatedMoney >= campaignCost && TownHallUIManager.Instance.SensibilizationCampaigns[this] < maxTimesDone && !activeCampaignGO.activeInHierarchy)
            {
                doCampaignButton.interactable = true;
            }
            else
            {
                doCampaignButton.interactable = false;
            }
        }


        protected virtual void OpenDescription()
        {
            TownHallUIManager.Instance.CloseDescriptions();
            if (!campaignDescription.activeInHierarchy) campaignDescription.SetActive(true);
        }


        public void CloseDescription()
        {
            if (campaignDescription.activeInHierarchy) campaignDescription.SetActive(false);
        }


        public void DoCampaign()
        {
            EconomyManager.Instance.UpdateMoney(campaignCost);

            TownHallUIManager.Instance.CampaignApproved(this);
            doCampaignButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Do Campaign (" + TownHallUIManager.Instance.SensibilizationCampaigns[this] + "/" + maxTimesDone + ")";

            TownHall.Instance.StartCoroutine(TownHall.Instance.ShowCampaignProgression(this, ClockUI.Instance.day, ClockUI.Instance.day + campaignDuration * 2.0f));

            if (SensCampaign == SensibilizationCampaignOptions.NewDisease)
            {
                foreach(Entity person in PopulationManager.Instance.Population)
                {
                    person.Feelings.UpdateDiseaseFear(Random.Range(0.05f, 0.20f));
                }
            }
            else if (SensCampaign == SensibilizationCampaignOptions.MaskUse)
            {
                foreach (Entity person in PopulationManager.Instance.Population)
                {
                    float prob = Random.value;
                    if(person.Feelings.DiseaseFear - 0.50f < prob)
                    {
                        if (person.Disease != null)
                        {
                            float multiplier = person.Disease.GetHostTraitMultiplier("diseaseRadius");
                            person.Disease.ChangeTraitMultiplier("diseaseRadius", multiplier * 0.85f);
                        }
                    }
                }
            }
            else if (SensCampaign == SensibilizationCampaignOptions.HandDisinfection)
            {
                foreach (Entity person in PopulationManager.Instance.Population)
                {
                    float prob = Random.value;
                    if (person.Feelings.DiseaseFear - 10f < prob)
                    {
                        if (person.Disease != null)
                        {
                            float multiplier = person.Disease.GetHostTraitMultiplier("diseaseRate");
                            person.Disease.ChangeTraitMultiplier("diseaseRate", multiplier * 1.2f);
                            person.StopDiseaseTicks();
                            person.StartDiseaseTicks();
                        }
                    }
                }
            }
            else if (SensCampaign == SensibilizationCampaignOptions.SonnerTreatment)
            {
                //TODO
            }
            else if (SensCampaign == SensibilizationCampaignOptions.ApprovalRatingBoost)
            {
                foreach (Entity person in PopulationManager.Instance.Population)
                {
                    person.Feelings.UpdateApprovalRating(Random.Range(0.05f, 0.15f));
                }
            }
            activeCampaignGO.SetActive(true);
            activeCampaignGO.GetComponent<RawImage>().color = new Color(1, 1, 1, 0.25f);

        }
    }
}
