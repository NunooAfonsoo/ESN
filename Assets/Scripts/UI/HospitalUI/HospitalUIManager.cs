using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Assets.Scripts.Managers;
using Assets.Scripts.Buildings;
using System;
using Assets.Scripts.Citizens;

namespace Assets.Scripts.UI.HospitalUI
{
    public class HospitalUIManager : MonoBehaviour
    {
        public static HospitalUIManager Instance;

        public GameObject HospitalIntroText;

        [SerializeField] private Button ExitButton;
        [SerializeField] private Button HelpButton;

        [SerializeField] private Button ExpandNUButton;
        [SerializeField] private Button DecreaseNUButton;
        [SerializeField] private TextMeshProUGUI NUText;
        [SerializeField] private Button ExpandICUButton;
        [SerializeField] private Button DecreaseICUButton;
        [SerializeField] private TextMeshProUGUI ICUText;

        [SerializeField] private GameObject EmptyNormalUnitText;
        [SerializeField] private GameObject EmptyICUText;

        [SerializeField] private GameObject PersonInfoNUTemplate;
        public Dictionary<string, GameObject> PersonInfoNUDict;

        [SerializeField] private GameObject PersonInfoICUTemplate;
        public Dictionary<string, GameObject> PersonInfoICUDict;

        [SerializeField] private TabGroup tabGroup;

        private int increaseCapacityCost;
        private int decreaseCapacityCost;

        private void Awake()
        {
            Instance = this;
            increaseCapacityCost = -100;
            decreaseCapacityCost = -50;
            PersonInfoNUDict = new Dictionary<string, GameObject>();
            PersonInfoICUDict = new Dictionary<string, GameObject>();

            HelpButton.onClick.AddListener(delegate { ResetMenu(); } );
            ExitButton.onClick.AddListener(delegate { ExitMenu(); } );

            ExpandNUButton.onClick.AddListener(delegate { UpdateUnitsOccupationCapacity(Hospital.HospitalUnit.NormalUnit, 0, 5, increaseCapacityCost); });
            ExpandICUButton.onClick.AddListener(delegate { UpdateUnitsOccupationCapacity(Hospital.HospitalUnit.IntensiveCareUnit, 0, 2, increaseCapacityCost); });

            DecreaseNUButton.onClick.AddListener(delegate { UpdateUnitsOccupationCapacity(Hospital.HospitalUnit.NormalUnit, 0, -5, decreaseCapacityCost); });
            DecreaseICUButton.onClick.AddListener(delegate { UpdateUnitsOccupationCapacity(Hospital.HospitalUnit.IntensiveCareUnit, 0, -2, decreaseCapacityCost); });


            Hospital.OnNormalUnitEntry += IcreaseHospitalCurrentOccupation;
            Hospital.OnICUEntry += IcreaseHospitalCurrentOccupation;
            Hospital.OnHospitalExit += DecreaseHospitalCurrentOccupation;

            
            gameObject.SetActive(false);
        }



        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                gameObject.SetActive(false);
                UIManager.Instance.ActiveMenu = null;
            }

            SetButtonInteractability("ExpandNUButton");
            SetButtonInteractability("ExpandICUButton");
            SetButtonInteractability("DecreaseNUButton");
            SetButtonInteractability("DecreaseICUButton");
        }

        void IcreaseHospitalCurrentOccupation(Hospital.HospitalUnit unit)
        {
            UpdateUnitsOccupationCapacity(unit, 1);
        }

        void DecreaseHospitalCurrentOccupation(Hospital.HospitalUnit unit)
        {
            UpdateUnitsOccupationCapacity(unit, -1);
        }



        private void ChangePatientsUnit(PersonInfo personInfo, Hospital.HospitalUnit newHospitalUnit)
        {
            switch (newHospitalUnit)
            {
                case Hospital.HospitalUnit.NormalUnit:
                    DecreaseHospitalCurrentOccupation(Hospital.HospitalUnit.IntensiveCareUnit);
                    RemovePersonInfo(personInfo.Person, Hospital.HospitalUnit.IntensiveCareUnit);
                    Hospital.Instance.TryEnterBuilding(personInfo.Person, Hospital.HospitalUnit.NormalUnit);


                    break;
                case Hospital.HospitalUnit.IntensiveCareUnit:
                    DecreaseHospitalCurrentOccupation(Hospital.HospitalUnit.NormalUnit);

                    RemovePersonInfo(personInfo.Person, Hospital.HospitalUnit.NormalUnit);
                    Hospital.Instance.TryEnterBuilding(personInfo.Person, Hospital.HospitalUnit.IntensiveCareUnit);

                    //personInfo.person.StopCoroutine("Heal");

                    break;
            }
        }

        public void SetEmptyUnitText(string unitName, bool empty)
        {
            if(empty)
            {
                if(unitName == "NormalUnit")
                {
                    EmptyNormalUnitText.SetActive(true);
                }
                else if(unitName == "ICU")
                {
                    EmptyICUText.SetActive(true);
                }
            }
            else
            {
                if(unitName == "NormalUnit")
                {
                    EmptyNormalUnitText.SetActive(false);
                }
                else if(unitName == "ICU")
                {
                    EmptyICUText.SetActive(false);
                }
            }
        }



        public int GetIncreaseCapacityCost()
        {
            return increaseCapacityCost;
        }


        public int GetDecreaseCapacityCost()
        {
            return decreaseCapacityCost;
        }


        public void SetIncreaseCapacityCost(int increaseCapacityCost)
        {
            this.increaseCapacityCost = increaseCapacityCost;
            string buttonText = "+5 Capacity (" + Mathf.Abs(increaseCapacityCost).ToString() + "$)";
            SetButtonText(ExpandNUButton, buttonText);
            buttonText = "+2 Capacity (" + Mathf.Abs(increaseCapacityCost).ToString() + "$)";
            SetButtonText(ExpandICUButton, buttonText);
        }


        public void SetDecreaseCapacityCost(int decreaseCapacityCost)
        {
            this.decreaseCapacityCost = decreaseCapacityCost;
            string buttonText = "-5 Capacity (" + Mathf.Abs(decreaseCapacityCost).ToString() + "$)";
            SetButtonText(DecreaseNUButton, buttonText);
            buttonText = "-2 Capacity (" + Mathf.Abs(decreaseCapacityCost).ToString() + "$)";
            SetButtonText(DecreaseICUButton, buttonText);
        }


        private void SetButtonText(Button button, string text)
        {
            button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        }


        public void CreateNewPersonInfo(Entity person, Hospital.HospitalUnit unit)
        {
            GameObject newInstance;
            switch(unit)
            {
                case Hospital.HospitalUnit.NormalUnit:
                    newInstance = GameObject.Instantiate(PersonInfoNUTemplate);
                    newInstance.transform.SetParent(PersonInfoNUTemplate.transform.parent);
                    newInstance.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = person.name;
                    newInstance.SetActive(true);
                    newInstance.transform.localScale = new Vector3(1, 1, 1);
                    PersonInfoNUDict.Add(person.name, newInstance);

                    newInstance.GetComponent<PersonInfo>().Unit = Hospital.HospitalUnit.NormalUnit;
                    newInstance.GetComponent<PersonInfo>().Person = person;
                    newInstance.GetComponent<PersonInfo>().OnUnitChange += ChangePatientsUnit;

                    SetEmptyUnitText("NormalUnit", false);
                    break;

                case Hospital.HospitalUnit.IntensiveCareUnit:
                    newInstance = GameObject.Instantiate(PersonInfoICUTemplate);
                    newInstance.transform.SetParent(PersonInfoICUTemplate.transform.parent);
                    newInstance.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = person.name;
                    newInstance.SetActive(true);
                    newInstance.transform.localScale = new Vector3(1, 1, 1);
                    PersonInfoICUDict.Add(person.name, newInstance);

                    newInstance.GetComponent<PersonInfo>().Unit = Hospital.HospitalUnit.IntensiveCareUnit;
                    newInstance.GetComponent<PersonInfo>().Person = person;
                    newInstance.GetComponent<PersonInfo>().OnUnitChange += ChangePatientsUnit;

                    SetEmptyUnitText("ICU", false);
                    break;
            }
        }


        public void RemovePersonInfo(Entity person, Hospital.HospitalUnit unit)
        {
            switch (unit)
            {
                case Hospital.HospitalUnit.NormalUnit:
                    Destroy(PersonInfoNUDict[person.name]);
                    PersonInfoNUDict.Remove(person.name);
                    Hospital.Instance.peopleInNU.Remove(person);
                    if(Hospital.Instance.peopleInNU.Count == 0) SetEmptyUnitText("NormalUnit", true);
                    break;

                case Hospital.HospitalUnit.IntensiveCareUnit:
                    Destroy(PersonInfoICUDict[person.name]);
                    PersonInfoICUDict.Remove(person.name);
                    Hospital.Instance.peopleInICU.Remove(person);
                    if (Hospital.Instance.peopleInICU.Count == 0) SetEmptyUnitText("ICU", true);
                    break;
            }
        }


        private void SetButtonInteractability(string button)
        {
            float money = EconomyManager.Instance.UpdatedMoney;

            switch (button)
            {
                case "ExpandNUButton":
                    if(money < Mathf.Abs(increaseCapacityCost) && ExpandNUButton.interactable)
                    {
                        ExpandNUButton.interactable = false;
                    }
                    else if(money > Mathf.Abs(increaseCapacityCost) && !ExpandNUButton.interactable)
                    {
                        ExpandNUButton.interactable = true;
                    }
                    break;
                case "ExpandICUButton":
                    if(money < Mathf.Abs(increaseCapacityCost) && ExpandICUButton.interactable)
                    {
                        ExpandICUButton.interactable = false;
                    }
                    else if(money > Mathf.Abs(increaseCapacityCost) && !ExpandICUButton.interactable)
                    {
                        ExpandICUButton.interactable = true;
                    }
                    break;
                case "DecreaseNUButton":
                    int hospitalNUCap = Hospital.Instance.GetNormalUnitCapacity();
                    int hospitalNUOccupation = Hospital.Instance.GetCurrentNUOccupation();

                    if((money < Mathf.Abs(decreaseCapacityCost) && DecreaseNUButton.interactable) || (money > Mathf.Abs(decreaseCapacityCost) && DecreaseNUButton.interactable && hospitalNUCap - 5 < hospitalNUOccupation))
                    {
                        DecreaseNUButton.interactable = false;
                    }
                    else if(money > Mathf.Abs(decreaseCapacityCost) && !DecreaseNUButton.interactable && hospitalNUCap - 5 >= hospitalNUOccupation)
                    {
                        DecreaseNUButton.interactable = true;
                    }
                    break;
                case "DecreaseICUButton":
                    int hospitalICUCap = Hospital.Instance.GetICUCapacity();
                    int hospitalICUccupation = Hospital.Instance.GetCurrentICUOccupation();

                    if((money < Mathf.Abs(decreaseCapacityCost) && DecreaseICUButton.interactable) || (money > Mathf.Abs(decreaseCapacityCost) && DecreaseICUButton.interactable && hospitalICUCap - 2 < hospitalICUccupation))
                    {
                        DecreaseICUButton.interactable = false;
                    }
                    else if(money > Mathf.Abs(decreaseCapacityCost) && !DecreaseICUButton.interactable && hospitalICUCap - 2 >= hospitalICUccupation)
                    {
                        DecreaseICUButton.interactable = true;
                    }
                    break;
            }
        }

        private void UpdateUnitsOccupationCapacity(Hospital.HospitalUnit unit, int occupationChange = 0, int changeCapacity = 0, int capacityChangeCost = 0)
        {
            if(changeCapacity != 0)
            {
                float money = EconomyManager.Instance.UpdatedMoney;

                if(money >= Mathf.Abs(capacityChangeCost))
                {
                    if(unit == Hospital.HospitalUnit.NormalUnit)
                    {
                        EconomyManager.Instance.UpdateMoney(capacityChangeCost);
                        Hospital.Instance.ChangeNormalUnitCapacity(changeCapacity);
                        UpdateNUText();
                    }
                    else if(unit == Hospital.HospitalUnit.IntensiveCareUnit)
                    {
                        EconomyManager.Instance.UpdateMoney(capacityChangeCost);
                        Hospital.Instance.ChangeICUCapacity(changeCapacity);
                        UpdateICUText();
                    }
                }
            }
            else
            {
                if(unit == Hospital.HospitalUnit.NormalUnit)
                {
                    Hospital.Instance.ChangeNormalUnitOccupation(occupationChange);
                    UpdateNUText();
                }
                else if(unit == Hospital.HospitalUnit.IntensiveCareUnit)
                {
                    Hospital.Instance.ChangeICUOccupation(occupationChange);
                    UpdateICUText();
                }
            }
        }




        public void UpdateNUText()
        {
            NUText.text = "Normal Care Unit Capapcity " + Hospital.Instance.GetCurrentNUOccupation() + "/" + Hospital.Instance.GetNormalUnitCapacity();
        }

        public void UpdateICUText()
        {
            ICUText.text = "Intensive Care Unit Capapcity " + Hospital.Instance.GetCurrentICUOccupation() + " / " + Hospital.Instance.GetICUCapacity();
        }

        private void OnEnable()
        {
            ResetMenu();
        }

        private void OnDestroy()
        {
            Hospital.OnNormalUnitEntry -= IcreaseHospitalCurrentOccupation;
            Hospital.OnICUEntry -= IcreaseHospitalCurrentOccupation;
            Hospital.OnHospitalExit -= DecreaseHospitalCurrentOccupation;
        }

        public void ResetMenu()
        {
            HospitalIntroText.SetActive(true);

            foreach(string personName in PersonInfoNUDict.Keys)
            {
                PersonInfoNUDict[personName].SetActive(false);
            }

            foreach (string personName in PersonInfoICUDict.Keys)
            {
                PersonInfoICUDict[personName].SetActive(false);
            }

            tabGroup.ResetTabs();
        }

        private void ExitMenu()
        {
            UIManager.Instance.ActiveMenu = null;
            gameObject.SetActive(false);
        }

    }

}