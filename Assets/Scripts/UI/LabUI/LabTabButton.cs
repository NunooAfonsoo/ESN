using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Assets.Scripts.Managers;



namespace Assets.Scripts.UI.LabUI
{
    public class LabTabButton : TabButton
    {

        public enum LabTab
        {
            Scientists,
            CureTree,
            TreatmentTree
        }

        public LabTab Tab;
        public GameObject TabObject;

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            LabUIManager.Instance.LabIntroText.SetActive(false);


            if (Tab == LabTab.Scientists)
            {
                if(PopulationManager.Instance.Scientists.Count == 0)
                {
                    LabUIManager.Instance.EmptyTabText.SetActive(true);
                }
                else
                {
                    if(LabUIManager.Instance.EmptyTabText.activeInHierarchy) LabUIManager.Instance.EmptyTabText.SetActive(false);

                    foreach (string key in LabUIManager.Instance.ScientistInfoDict.Keys)
                    {
                        LabUIManager.Instance.ScientistInfoDict[key].SetActive(true);
                    }
                }
            }
            tabGroup.ResetOtherTabs(this);
            TabObject.SetActive(true);
        }


        public override void ResetTab()
        {
            base.ResetTab();
            if(Tab == LabTab.CureTree)
            {
                foreach(CureTreeNode node in TabObject.GetComponent<CureTechTree>().treeNodes)
                {
                    node.CloseDescription();
                }
            }
            else if (Tab == LabTab.TreatmentTree)
            {
                foreach (TreatmentTreeNode node in TabObject.GetComponent<TreatmentTechTree>().treeNodes)
                {
                    node.CloseDescription();
                }
            }

            TabObject?.SetActive(false);
        }
    }
}