using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Scripts.Managers;


namespace Assets.Scripts.UI.TownHallUI
{
    public class BudgetOverview : MonoBehaviour
    {
        [SerializeField] private GameObject loanButtonGO;
        [SerializeField] private GameObject budgetButtonGO;
        [SerializeField] private GameObject goBackButtonGO;
        [SerializeField] private GameObject budgetOverviewGO;

        [SerializeField] private TextMeshProUGUI Income;
        [SerializeField] private TextMeshProUGUI ScientistsSalaries;
        [SerializeField] private TextMeshProUGUI LabUpkeep;
        [SerializeField] private TextMeshProUGUI HospitalUpkeep;
        [SerializeField] private TextMeshProUGUI LoanFees;
        [SerializeField] private TextMeshProUGUI Total;

        private Button budgetOverviewButton;

        private void Awake()
        {
            budgetOverviewButton = transform.GetChild(0).GetComponent<Button>();
            budgetOverviewButton.onClick.AddListener(delegate { OpenBudgetOverview(); });
            goBackButtonGO.GetComponent<Button>().onClick.AddListener(delegate { ResetBudget(); });
        }


        public void UpdateBudget()
        {
            if (budgetOverviewGO.activeInHierarchy) OpenBudgetOverview();
        }

        public void OpenBudgetOverview()
        {
            TownHallUIManager.Instance.CloseDescriptions();

            loanButtonGO.SetActive(false);
            budgetButtonGO.SetActive(false);
            goBackButtonGO.SetActive(true);
            budgetOverviewGO.SetActive(true);
            Income.text = EconomyManager.Instance.LastIncome.ToString("F2");
            ScientistsSalaries.text = EconomyManager.Instance.ScientistsSalaries.ToString("F2");
            LabUpkeep.text = EconomyManager.Instance.LabUpkeep.ToString("F2");
            HospitalUpkeep.text = EconomyManager.Instance.HospitalUpkeep.ToString("F2");
            LoanFees.text = EconomyManager.Instance.LoanFees.ToString("F2");

            Total.text = (EconomyManager.Instance.LastIncome + EconomyManager.Instance.ScientistsSalaries + EconomyManager.Instance.LabUpkeep + EconomyManager.Instance.HospitalUpkeep + EconomyManager.Instance.LoanFees).ToString("F2");
        }


        public void ResetBudget()
        {
            loanButtonGO.SetActive(true);
            budgetButtonGO.SetActive(true);
            goBackButtonGO.SetActive(false);
            budgetOverviewGO.SetActive(false);
        }
    }
}
