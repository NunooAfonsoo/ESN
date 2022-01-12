using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Managers;



namespace Assets.Scripts.UI.LabUI
{
    public class Node : MonoBehaviour
    {
        [SerializeField] protected List<Node> parents;
        [SerializeField] protected List<Node> children;
        [SerializeField] protected int researchMoneyCost;
        [SerializeField] protected int researchSPCost;
        [SerializeField] protected Button researchTechButton;
        [SerializeField] protected Button nodeButton;

        public string NodeName;
        [SerializeField] protected GameObject nodeDescription;


        private void Awake()
        {
            nodeButton = transform.GetChild(1).GetComponent<Button>();
            nodeButton.onClick.AddListener(delegate { OpenDescription(); });
        }





        protected virtual void OpenDescription()
        {
            if (!nodeDescription.activeInHierarchy) nodeDescription.SetActive(true);
        }

        public void CloseDescription()
        {
            if (nodeDescription.activeInHierarchy) nodeDescription.SetActive(false);
        }


        public virtual void ResearchTech()
        {
            EconomyManager.Instance.UpdateMoney(-researchMoneyCost);
            EconomyManager.Instance.UpdateScience(-researchSPCost);
            
        }
    }
}