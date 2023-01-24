using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Managers;
using Assets.Scripts.Buildings;
using Assets.Scripts.Citizens;
using TMPro;


namespace Assets.Scripts.UI.TownHallUI
{
    public class TownHallUIManager : MonoBehaviour
    {
        public static TownHallUIManager Instance;

        public GameObject TownHallIntroText;

        public Button ExitButton;
        public Button HelpButton;


        [SerializeField] private TabGroup tabGroup;

        [SerializeField] private Slider workTimeSlider;
        [SerializeField] private TextMeshProUGUI workTimeText;

        public Dictionary<SensibilizationCampaign, int> SensibilizationCampaigns; //number of times campaign is done
        public Dictionary<Law, int> Laws;
        [SerializeField] private Loan loan;
        public BudgetOverview budgetOverview;


        public Law.LawOptions ActiveLaw;

        public ActiveLaw ActiveLawGO;

        private void Awake()
        {
            Instance = this;

            HelpButton.onClick.AddListener(delegate { ResetMenu(); });
            ExitButton.onClick.AddListener(delegate { ExitMenu(); });

            workTimeSlider.onValueChanged.AddListener(delegate { ChangeWorkTime(); });


            ActiveLaw = Law.LawOptions.NoLawActive;

            SensibilizationCampaigns = new Dictionary<SensibilizationCampaign, int>();
            Laws = new Dictionary<Law, int>();
        }

        private void Start()
        {
            ActiveLawGO = GameObject.Find("LawActive").GetComponent<ActiveLaw>();
            ActiveLawGO.gameObject.SetActive(false);

            gameObject.SetActive(false);
        }

        internal int GetSensibilizationTimes(SensibilizationCampaign.SensibilizationCampaignOptions sensibilizationCampaign)
        {
            foreach (SensibilizationCampaign sensCampaign in SensibilizationCampaigns.Keys)
            {
                if (sensCampaign.SensCampaign == sensibilizationCampaign) return SensibilizationCampaigns[sensCampaign];
            }
            return 0;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                gameObject.SetActive(false);
                UIManager.Instance.ActiveMenu = null;
            }

        }

        private void SetButtonInteractability(string button)
        {
        }

        public void RegisterCampaign(SensibilizationCampaign sensCampaign)
        {
            SensibilizationCampaigns.Add(sensCampaign, 0);
        }


        public void RegisterLaw(Law law)
        {
            Laws.Add(law, 0);
        }

        public void CampaignApproved(SensibilizationCampaign sensCampaign)
        {
            SensibilizationCampaigns[sensCampaign]++;
        }


        public void LawEnacted(Law law)
        {
            Laws[law]++;
        }


        private void ChangeWorkTime()
        {
            float workshift = 0;
            foreach(Entity person in PopulationManager.Instance.Population)
            {
                /*
                person.Workshift = 0.5f + workTimeSlider.value * 0.3f;
                workshift = person.Workshift;
                person.Feelings.UpdateHappiness(person.Workshift);
                */
                workshift = (2f * workTimeSlider.value + 6f);
                person.Workshift = workshift / 12f;
                person.Feelings.UpdateHappiness(person.Workshift);
            }
            workTimeText.text = "~ " + (workshift).ToString("F0") + " hrs";
        }

        public void CloseDescriptions()
        {
            foreach(Law law in Laws.Keys)
            {
                law.CloseDescription();
            }

            foreach(SensibilizationCampaign sensCampaign in SensibilizationCampaigns.Keys)
            {
                sensCampaign.CloseDescription();
            }

            loan.CloseDescription();

            budgetOverview.ResetBudget();            
        }


        public void OnEnable()
        {
            ResetMenu();
        }


        public void ResetMenu()
        {
            TownHallIntroText.SetActive(true);

            tabGroup.ResetTabs();
        }

        private void ExitMenu()
        {
            UIManager.Instance.ActiveMenu = null;
            gameObject.SetActive(false);
        }
    }
}