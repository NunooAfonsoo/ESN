using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Managers;
using Assets.Scripts.Buildings;
using Assets.Scripts.Citizens;
using TMPro;


namespace Assets.Scripts.UI.LabUI
{
    public class LabUIManager : MonoBehaviour
    {
        public static LabUIManager Instance;

        public GameObject LabIntroText;

        public Button ExitButton;
        public Button HelpButton;

        public Button HireNewScientist;
        [SerializeField] private GameObject ScientistInfoTemplate;
        public Dictionary<string, GameObject> ScientistInfoDict;

        public GameObject EmptyTabText;

        [SerializeField] private TabGroup tabGroup;

        [SerializeField]  private float scientistSalary;

        private void Awake()
        {
            Instance = this;
            ScientistInfoDict = new Dictionary<string, GameObject>();

            HelpButton.onClick.AddListener(delegate { ResetMenu(); });
            ExitButton.onClick.AddListener(delegate { ExitMenu(); });

            HireNewScientist.onClick.AddListener(delegate { HireScientist(-150); });


            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                gameObject.SetActive(false);
                UIManager.Instance.ActiveMenu = null;
            }

            SetButtonInteractability("HireNewScientist");
        }



        private void SetButtonInteractability(string button)
        {
            switch(button)
            {
                case "HireNewScientist":
                    if(EconomyManager.Instance.UpdatedMoney >= 150) HireNewScientist.interactable = true;
                    else HireNewScientist.interactable = false;
                    break;
            }
        }




        private void HireScientist(int hireCost)
        {
            float money = EconomyManager.Instance.UpdatedMoney;

            if (money >= Mathf.Abs(hireCost))
            {
                EconomyManager.Instance.UpdateMoney(hireCost);
                GameObject newInstance = GameObject.Instantiate(PopulationManager.Instance.ScientistsModels[UnityEngine.Random.Range(0, PopulationManager.Instance.ScientistsModels.Length)]);

                GameObject newScientistInfo = GameObject.Instantiate(ScientistInfoTemplate);
                newInstance.name += UnityEngine.Random.value.ToString();
                newScientistInfo.name = newInstance.name;
                newScientistInfo.transform.SetParent(ScientistInfoTemplate.transform.parent);
                newScientistInfo.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = newScientistInfo.name;
                newScientistInfo.SetActive(true);
                newScientistInfo.transform.localScale = new Vector3(1, 1, 1);

                newScientistInfo.GetComponent<ScientistInfo>().Scientist = newInstance.GetComponent<Scientist>();


                newScientistInfo.GetComponent<ScientistInfo>().OnScientistFired += UIManager.Instance.FiredScientist;

                ScientistInfoDict.Add(newInstance.name, newScientistInfo);
                EmptyTabText.SetActive(false);

                EconomyManager.Instance.UpdateScientistsSalaries(scientistSalary);
            }
        }


        public void FireScientist(ScientistInfo scientistInfo)
        {
            if (PopulationManager.Instance.Scientists.Count == 0) EmptyTabText.SetActive(true);
            scientistInfo.Scientist.GetFired();

            Destroy(scientistInfo.gameObject);
            ScientistInfoDict[scientistInfo.Scientist.name].GetComponent<ScientistInfo>().OnScientistFired -= UIManager.Instance.FiredScientist;
            ScientistInfoDict.Remove(scientistInfo.Scientist.name);
        }

        private void OnEnable()
        {
            ResetMenu();
        }


        public void ResetMenu()
        {
            LabIntroText.SetActive(true);

            tabGroup.ResetTabs();
        }

        private void ExitMenu()
        {
            UIManager.Instance.ActiveMenu = null;
            gameObject.SetActive(false);
        }


    }




}