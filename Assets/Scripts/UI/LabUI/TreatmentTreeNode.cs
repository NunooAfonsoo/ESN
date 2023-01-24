using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Buildings;
using Assets.Scripts.UI.HospitalUI;
using Assets.Scripts.Managers;


namespace Assets.Scripts.UI.LabUI
{
    public class TreatmentTreeNode : Node
    {
        private enum NodeResearch
        {
            NewVentilators,
            NewPainkillers,
            ImprovementsOnVentilations,
            ECMO,
            ExpandHospital,
            CheaperBedMaterials,
            OxygenTherapy

        }

        [SerializeField] private NodeResearch nodeResearch;


        private void Update()
        {
            CheckNodeRequirements();

            if (TreatmentTechTree.Instance.researchedTreeNodes.Contains(this))
            {
                nodeButton.targetGraphic.color = new Color(0.37f, 1f, 0.37f, 1f);
            }

            if (nodeDescription.activeInHierarchy)
            {
                if (EconomyManager.Instance.UpdatedMoney < researchMoneyCost || EconomyManager.Instance.Science < researchSPCost || TreatmentTechTree.Instance.researchedTreeNodes.Contains(this)) researchTechButton.interactable = false;
                else researchTechButton.interactable = true;
            }
        }



        private void CheckNodeRequirements()
        {
            if (EconomyManager.Instance.UpdatedMoney >= researchMoneyCost && EconomyManager.Instance.Science >= researchSPCost)
            {
                nodeButton.targetGraphic.color = new Color(1f, 1f, 1f, 1f);
                foreach (TreatmentTreeNode node in parents)
                {
                    if (!TreatmentTechTree.Instance.researchedTreeNodes.Contains(node))
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
            TreatmentTechTree.Instance.CloseDescriptions();
            base.OpenDescription();
        }



        public override void ResearchTech()
        {
            base.ResearchTech();
            TreatmentTechTree.Instance.researchedTreeNodes.Add(this);


            if (nodeResearch == NodeResearch.NewVentilators)
            {
                Hospital.Instance.UpgradeNU(1.1f);
                Hospital.Instance.UpgradeICU(1.1f);
            }
            else if (nodeResearch == NodeResearch.NewPainkillers)
            {
                Hospital.Instance.UpgradeNU(1.15f);
                Hospital.Instance.UpgradeICU(1.15f);
            }
            else if (nodeResearch == NodeResearch.ImprovementsOnVentilations)
            {
                Hospital.Instance.UpgradeNUTimeStep(0.80f);
                Hospital.Instance.UpgradeICUTimeStep(0.80f);
            }
            else if (nodeResearch == NodeResearch.ECMO)
            {
                Hospital.Instance.UpgradeICU(2f);
            }
            else if (nodeResearch == NodeResearch.ExpandHospital)
            {
                Hospital hospital = Hospital.Instance;
                HospitalUIManager hospitalUIManager = HospitalUIManager.Instance;

                int NUCap = (int)(hospital.GetNormalUnitCapacity() * 0.5f);
                hospital.ChangeNormalUnitCapacity(NUCap);
                hospitalUIManager.UpdateNUText();

                int ICUCap = (int)(hospital.GetICUCapacity() * 0.5f);
                hospital.ChangeICUCapacity(ICUCap);
                hospitalUIManager.UpdateICUText();

            }
            else if (nodeResearch == NodeResearch.CheaperBedMaterials)
            {
                Hospital hospital = Hospital.Instance;
                HospitalUIManager hospitalUIManager = HospitalUIManager.Instance;

                int increaseCapacityCost = hospitalUIManager.GetIncreaseCapacityCost();
                hospitalUIManager.SetIncreaseCapacityCost((int)(increaseCapacityCost * 0.66f));

                int decreaseCapacityCost = hospitalUIManager.GetDecreaseCapacityCost();
                hospitalUIManager.SetDecreaseCapacityCost((int)(decreaseCapacityCost * 0.66f));
            }
            else if (nodeResearch == NodeResearch.OxygenTherapy)
            {
                Hospital.Instance.ImproveProbOfTreatmentSuccess(1.17f);
            }
        }

    }
}

