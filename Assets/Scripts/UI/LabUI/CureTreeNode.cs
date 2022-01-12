using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Citizens;
using Assets.Scripts.Managers;


namespace Assets.Scripts.UI.LabUI
{
    public class CureTreeNode : Node
    {

        private enum NodeResearch
        {
            TrialsOnAnimals,
            TrialsOnHumans,
            FirstVaccineRevision,
            SecondVaccineRevision,
            VaccinationProcess
        }

        [SerializeField] private NodeResearch nodeResearch;


        private void Update()
        {
            CheckNodeRequirements();

            if (CureTechTree.Instance.researchedTreeNodes.Contains(this))
            {
                nodeButton.targetGraphic.color = new Color(0.37f, 1f, 0.37f, 1f);
            }

            if (nodeDescription.activeInHierarchy)
            {
                if (EconomyManager.Instance.UpdatedMoney < researchMoneyCost || EconomyManager.Instance.Science < researchSPCost || CureTechTree.Instance.researchedTreeNodes.Contains(this)) researchTechButton.interactable = false;
                else researchTechButton.interactable = true;
            }
        }



        private void CheckNodeRequirements()
        {
            if (EconomyManager.Instance.UpdatedMoney >= researchMoneyCost && EconomyManager.Instance.Science >= researchSPCost)
            {
                nodeButton.targetGraphic.color = new Color(1f, 1f, 1f, 1f);
                foreach (CureTreeNode node in parents)
                {
                    if (!CureTechTree.Instance.researchedTreeNodes.Contains(node))
                    {
                        nodeButton.targetGraphic.color = new Color(1f, 0.37f, 0.37f, 1f);
                    }
                }
            }
            else
            {
                nodeButton.targetGraphic.color = new Color(1f, 0.37f, 0.37f, 1f);
            }
        }

        protected override void OpenDescription()
        {
            CureTechTree.Instance.CloseDescriptions();
            base.OpenDescription();
        }


        public override void ResearchTech()
        {
            base.ResearchTech();
            CureTechTree.Instance.researchedTreeNodes.Add(this);


            if (nodeResearch == NodeResearch.TrialsOnHumans)
            {
                int nPeople = 0;

                foreach(Entity person in PopulationManager.Instance.Population)
                {
                    if(person.Health > 40)
                    {
                        int waitTime = Random.Range(5, 30);
                        person.InvokeRepeating("SetTrialsOnHumans", waitTime, 1f);
                        nPeople++;
                        if (nPeople > PopulationManager.Instance.Population.Count / 10) break;
                    }
                }
            }
            else if (nodeResearch == NodeResearch.VaccinationProcess)
            {
                foreach (Entity person in PopulationManager.Instance.Population)
                {
                    int waitTime = Random.Range(5, 30);
                    person.InvokeRepeating("SetMassVaccination", waitTime, 1f);
                }
            }
        }
    }
}
