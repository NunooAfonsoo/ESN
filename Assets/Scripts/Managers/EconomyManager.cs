using UnityEngine;
using Assets.Scripts.Citizens;
using Assets.Scripts.UI.TownHallUI;
using System;

namespace Assets.Scripts.Managers
{
    public class EconomyManager : MonoBehaviour
    {
        public static EconomyManager Instance;

        public float UpdatedMoney;
        public int Science;

        public static event Action<float, float> OnMoneyChanged;
        public static event Action<int> OnScienceChanged;


        public int moneyUpdatesReceived;
        public float LastIncome;
        private float income;
        public float ScientistsSalaries;
        public float LabUpkeep;
        public float HospitalUpkeep;

        public float LoanFees;
        public float LoanAmount;


        private void Awake()
        {
            Instance = this;

            UpdateMoney(UpdatedMoney, false);
            moneyUpdatesReceived = 0;
            Science = 0;

            income = 0;
            ScientistsSalaries = 0;
            LabUpkeep = 0;
            HospitalUpkeep = 0;
            LoanFees = 0;
        }


        public void UpdateIncome(float updateAmount)
        {
            moneyUpdatesReceived++;
            income += updateAmount;
            if (moneyUpdatesReceived == PopulationManager.Instance.Population.Count)
            {
                UpdateMoney(0, false);
                LastIncome = income;
                TownHallUIManager.Instance.budgetOverview.UpdateBudget();
                moneyUpdatesReceived = 0;
                income = 0;
            }
        }

        public void UpdateScientistsSalaries(float scientistsSalaries)
        {
            this.ScientistsSalaries += scientistsSalaries;
            TownHallUIManager.Instance.budgetOverview.UpdateBudget();
        }

        public void UpdateLabUpkeep(float labUpkeep)
        {
            this.LabUpkeep = labUpkeep;
            TownHallUIManager.Instance.budgetOverview.UpdateBudget();
        }

        public void UpdateHospitalUpkeep(float hospitalUpkeep)
        {
            this.HospitalUpkeep = hospitalUpkeep;
            TownHallUIManager.Instance.budgetOverview.UpdateBudget();
        }

        public void UpdateLoanFees(float loanFees)
        {
            this.LoanFees = loanFees;
            TownHallUIManager.Instance.budgetOverview.UpdateBudget();
        }


        private void RepayLoan()
        {
            LoanAmount += LoanFees;
            if (LoanAmount <= 0)
            {
                LoanFees = 0;
                TownHallUIManager.Instance.budgetOverview.UpdateBudget();
                CancelInvoke("RepayLoan");
            }
        }


        public void UpdateMoney(float updateAmount, bool moneySpent = true)
        {
            /*
            this.updateAmount += updateAmount;
            float previousMoney = UpdatedMoney;
            UpdatedMoney += this.updateAmount;
            OnMoneyChanged?.Invoke(previousMoney, UpdatedMoney, this.updateAmount >= 0);
            this.updateAmount = 0;
            */

            float previousMoney = UpdatedMoney;
            if (moneySpent) UpdatedMoney += updateAmount;
            else UpdatedMoney += income + ScientistsSalaries + LabUpkeep + HospitalUpkeep + LoanFees;

            OnMoneyChanged?.Invoke(previousMoney, UpdatedMoney);

            TownHallUIManager.Instance.budgetOverview.UpdateBudget();
        }

        public void UpdateScience(int UpdateAmount)
        {
            Science += UpdateAmount;
            OnScienceChanged?.Invoke(Science);
        }
    }
}
