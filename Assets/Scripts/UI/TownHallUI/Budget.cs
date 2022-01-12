using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Managers;
using System;

namespace Assets.Scripts.UI.TownHallUI
{
    public class Budget : MonoBehaviour
    {

        public enum BudgetTab
        {
            Loan,
            BudgetOverview
        }

        [SerializeField] private BudgetTab budgetTab;



        [SerializeField] private Button takeLoanButton;
        private Button budgetButton;

        [SerializeField] private GameObject loanDescription;


        [Header("BudgetOverview")]
        [SerializeField] private GameObject loanButtonGO;
        [SerializeField] private GameObject budgetButtonGO;

        private void Awake()
        {
            budgetButton = transform.GetChild(0).GetComponent<Button>();
            budgetButton.onClick.AddListener(delegate { OpenDescription(); });

        }



        protected virtual void OpenDescription()
        {
            TownHallUIManager.Instance.CloseDescriptions();
            if (budgetTab == BudgetTab.Loan && !loanDescription.activeInHierarchy) loanDescription.SetActive(true);

            else OpenBudgetOverview();
        }

        private void OpenBudgetOverview()
        {
            loanButtonGO.SetActive(false);
            budgetButtonGO.SetActive(false);
            
        }

        public void ResetBudget()
        {
            loanButtonGO.SetActive(true);
            budgetButtonGO.SetActive(true);

        }
    }
}
