using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Managers;
using TMPro;

namespace Assets.Scripts.UI.TownHallUI
{
    public class Loan : MonoBehaviour
    {
        [SerializeField] private Button takeLoanButton;
        [SerializeField] private Button repayLoanButton;
        private Button loanButton;

        [SerializeField] private GameObject loanDescription;

        private void Awake()
        {
            loanButton = transform.GetChild(0).GetComponent<Button>();
            loanButton.onClick.AddListener(delegate { OpenDescription(); });
            takeLoanButton.onClick.AddListener(delegate { TakeLoan(); } );
            repayLoanButton.onClick.AddListener(delegate { RepayLoan(); });
        }

        private void Update()
        {
            if (EconomyManager.Instance.LoanAmount > 0)
            {
                takeLoanButton.interactable = false;
                if(EconomyManager.Instance.LoanAmount <= EconomyManager.Instance.UpdatedMoney) repayLoanButton.interactable = true;
                else repayLoanButton.interactable = false;
                repayLoanButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Repay Loan(" + EconomyManager.Instance.LoanAmount.ToString() + "$)";
            }
            else
            {
                takeLoanButton.interactable = true;
                repayLoanButton.interactable = false;
                repayLoanButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Repay Loan";
            }

        }


        protected virtual void OpenDescription()
        {
            TownHallUIManager.Instance.CloseDescriptions();
            if (!loanDescription.activeInHierarchy) loanDescription.SetActive(true);
        }


        public void CloseDescription()
        {
            if (loanDescription.activeInHierarchy) loanDescription.SetActive(false);
        }


        private void TakeLoan()
        {
            EconomyManager.Instance.LoanAmount = 210;
            EconomyManager.Instance.UpdateLoanFees(-5);
            EconomyManager.Instance.UpdateMoney(200);
            EconomyManager.Instance.InvokeRepeating("RepayLoan", 0, 2f);
        }

        private void RepayLoan()
        {
            EconomyManager.Instance.UpdateMoney(-EconomyManager.Instance.LoanAmount);
            EconomyManager.Instance.LoanAmount = 0;
            EconomyManager.Instance.UpdateLoanFees(0);
            EconomyManager.Instance.CancelInvoke("RepayLoan");
        }

    }
}
