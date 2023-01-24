using System;
using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Citizens;

namespace Assets.Scripts.Managers
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;


        public TextMeshProUGUI MoneyText;
        public TextMeshProUGUI ScienceText;
        public TextMeshProUGUI PopulationText;
        public TextMeshProUGUI ScientistsText;
        public TextMeshProUGUI InfectedText;
        public TextMeshProUGUI DeathsText;


        public TextMeshProUGUI AvrgHappiness;
        public TextMeshProUGUI AvrgProductiviy;
        public TextMeshProUGUI AvrgDiseaseFear;
        public TextMeshProUGUI AvrgApprovalRating;

        public GameObject ActiveMenu;

        private void Awake()
        {
            Instance = this;
            ActiveMenu = null;
        }


        private void Start()
        {
            MoneyText = GameObject.Find("MoneyText").GetComponent<TextMeshProUGUI>();
            ScienceText = GameObject.Find("ScienceText").GetComponent<TextMeshProUGUI>();
            PopulationText = GameObject.Find("PopulationText").GetComponent<TextMeshProUGUI>();
            ScientistsText = GameObject.Find("ScientistsText").GetComponent<TextMeshProUGUI>();
            InfectedText = GameObject.Find("InfectedText").GetComponent<TextMeshProUGUI>();
            DeathsText = GameObject.Find("DeathsText").GetComponent<TextMeshProUGUI>();

            MoneyText.text = "Money\n" + EconomyManager.Instance.UpdatedMoney;
            ScienceText.text = "Science\n" + EconomyManager.Instance.Science;
            PopulationText.text = "Population\n" + (PopulationManager.Instance.Population.Count + PopulationManager.Instance.Scientists.Count).ToString();
            ScientistsText.text = "Scientists\n" + PopulationManager.Instance.Scientists.Count;
            InfectedText.text = "Infected\n" + PopulationManager.Instance.Infected.Count;
            DeathsText.text = "Deaths\n" + PopulationManager.Instance.Deaths;


            EconomyManager.OnMoneyChanged += UpdateMoneyText;
            EconomyManager.OnScienceChanged += UpdateScienceText;


            AvrgHappiness = GameObject.Find("Happiness").GetComponent<TextMeshProUGUI>();
            AvrgProductiviy = GameObject.Find("Productivity").GetComponent<TextMeshProUGUI>(); ;
            AvrgDiseaseFear = GameObject.Find("DiseaseFear").GetComponent<TextMeshProUGUI>(); ;
            AvrgApprovalRating = GameObject.Find("ApprovalRating").GetComponent<TextMeshProUGUI>();

            AvrgHappiness.text = "Happiness: " + PopulationManager.Instance.AverageHappiness.ToString("F2") + "/100";
            AvrgProductiviy.text = "Productivity: " + PopulationManager.Instance.AverageProductivity.ToString("F2") + "/100";
            AvrgDiseaseFear.text = "Disease Fear: " + PopulationManager.Instance.AverageDiseaseFear.ToString("F2") + "/100";
            AvrgApprovalRating.text = "Approval Rating: " + PopulationManager.Instance.AverageApprovalRating.ToString("F2") + "/100";

            PopulationManager.OnHappinessChanged += UpdateAvrgHappiness;
            PopulationManager.OnProductivityChanged += UpdateAvrgProductivity;
            PopulationManager.OnDiseaseFearChanged += UpdateAvrgDiseaseFear;
            PopulationManager.OnApprovalRatingChanged += UpdateAvrgApprovalRating;

        }


        public void FiredScientist(Scientist scientist)
        {
            if (ScientistsText != null)
            {
                UpdatePopulationNumbers((Entity)scientist, true);
                StartCoroutine(NegativeUpdateVisual(ScientistsText, Color.white, 0.5f));
                ScientistsText.text = "Scientists\n" + PopulationManager.Instance.Scientists.Count;
            }
        }


        public void UpdateInfectedNumbers(Entity person, bool infected)
        {
            if (InfectedText != null)
            {
                InfectedText.text = "Infected\n" + PopulationManager.Instance.Infected.Count;
                if (infected)
                {
                    StartCoroutine(NegativeUpdateVisual(InfectedText, new Color(0.74f, 0.01f, 0.00f), 0.5f));
                }
            }
        }



        public void UpdatePopulationNumbers(Entity person, bool death)
        {
            if (PopulationText != null)
            {
                if (death) StartCoroutine(NegativeUpdateVisual(PopulationText, Color.white, 0.5f));
                else StartCoroutine(PositiveUpdateVisual(PopulationText, Color.white, 0.5f));
                PopulationText.text = "Population\n" + (PopulationManager.Instance.Population.Count + PopulationManager.Instance.Scientists.Count).ToString();
            }
        }

        public void UpdateScientistsNumbers(Entity person, bool death)
        {
            if (ScientistsText != null)
            {
                if(death) StartCoroutine(NegativeUpdateVisual(ScientistsText, Color.white, 0.5f));
                else StartCoroutine(PositiveUpdateVisual(ScientistsText, Color.white, 0.5f));
                ScientistsText.text = "Scientists\n" + PopulationManager.Instance.Scientists.Count;
            }
        }


        public void UpdateDeathsNumbers(Entity person, bool death = true)
        {
            DeathsText.text = "Deaths\n" + PopulationManager.Instance.Deaths;
            InfectedText.text = "Infected\n" + PopulationManager.Instance.Infected.Count;
            StartCoroutine(NegativeUpdateVisual(DeathsText, new Color(0.74f, 0.01f, 0.00f), 0.5f));
        }


        private void UpdateMoneyText(float previousMoney, float updatedMoney)
        {
            MoneyText.text = "Money\n" + updatedMoney.ToString("F2");
            if(updatedMoney >= 0)
            {
                MoneyText.color = new Color(0.14f, 0.87f, 0.23f);
            }
            else
            {
                MoneyText.color = new Color(0.74f, 0.01f, 0.00f);
            }

            if(previousMoney < 0 && updatedMoney >= 0)
            {
                StartCoroutine(PositiveUpdateVisual(MoneyText, new Color(0.14f, 0.87f, 0.23f), 0.5f));
            }
            else if(previousMoney >= 0 && updatedMoney < 0)
            {
                StartCoroutine(NegativeUpdateVisual(MoneyText, new Color(0.74f, 0.01f, 0.00f), 0.5f));
            }
        }


        private void UpdateScienceText(int science)
        {
            ScienceText.text = "Science\n" + science.ToString();
        }



        private void UpdateAvrgHappiness(float newAvrg)
        {
            AvrgHappiness.text = "Happiness: " + newAvrg.ToString("F2") + "/100";
        }


        private void UpdateAvrgProductivity(float newAvrg)
        {
            AvrgProductiviy.text = "Productivity: " + newAvrg.ToString("F2") + "/100";
        }


        private void UpdateAvrgDiseaseFear(float newAvrg)
        {
            AvrgDiseaseFear.text = "Disease Fear: " + newAvrg.ToString("F2") + "/100";
        }


        private void UpdateAvrgApprovalRating(float newAvrg)
        {
            AvrgApprovalRating.text = "Approval Rating: " + newAvrg.ToString("F2") + "/100";
        }



        private IEnumerator PositiveUpdateVisual(TextMeshProUGUI text, Color colorToReturn, float waitTime)
        {
            text.color = new Color(0.14f, 0.87f, 0.23f);
            text.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            yield return new WaitForSecondsRealtime(waitTime);
            text.color = colorToReturn;
            text.transform.localScale = new Vector3(1, 1, 1);
        }

        private IEnumerator NegativeUpdateVisual(TextMeshProUGUI text, Color colorToReturn, float waitTime)
        {
            text.color = new Color(0.74f, 0.01f, 0.00f);
            text.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            yield return new WaitForSecondsRealtime(waitTime);
            text.color = colorToReturn;
            text.transform.localScale = new Vector3(1, 1, 1);
        }




        private void OnDestroy()
        {
            EconomyManager.OnMoneyChanged -= UpdateMoneyText;
            EconomyManager.OnScienceChanged -= UpdateScienceText;

            PopulationManager.OnHappinessChanged -= UpdateAvrgHappiness;
            PopulationManager.OnProductivityChanged -= UpdateAvrgProductivity;
            PopulationManager.OnDiseaseFearChanged -= UpdateAvrgDiseaseFear;
            PopulationManager.OnApprovalRatingChanged -= UpdateAvrgApprovalRating;


            foreach (Entity person in PopulationManager.Instance.Population)
            {
                person.OnInfectedOrHealed -= UpdateInfectedNumbers;
                person.OnDeath -= UpdatePopulationNumbers;
                person.OnDeath -= UpdateDeathsNumbers;
            }

            foreach (Scientist person in PopulationManager.Instance.Scientists)
            {
                person.OnInfectedOrHealed -= UpdateInfectedNumbers;
                person.OnDeath -= UpdatePopulationNumbers;
                person.OnDeath -= UpdateDeathsNumbers;
            }
        }
    }
}