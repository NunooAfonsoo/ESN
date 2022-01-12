using Assets.Scripts.Citizens;
using Assets.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Clock;
using System;
using System.Collections.Generic;
using Assets.Scripts.Buildings;

namespace Assets.Scripts.UI.TownHallUI
{
    public class Law : MonoBehaviour
    {
        [SerializeField] private int lawCost;

        [SerializeField] private Button enactLawButton;
        [SerializeField] private Button revokeLawButton;

        private Button lawButton;

        [SerializeField] private GameObject lawDescription;

        public string LawName;

        public enum LawOptions
        {
            Lockdown,
            RemoteWork,
            ForcedLabour,
            NoLawActive
        }

        public LawOptions LawOption;



        private void Awake()
        {
            lawButton = transform.GetChild(0).GetComponent<Button>();
            lawButton.onClick.AddListener(delegate { OpenDescription(); });

            TownHallUIManager.Instance.RegisterLaw(this);
        }


        private void Update()
        {
            CheckNodeRequirements();
        }


        private void CheckNodeRequirements()
        {
            if (EconomyManager.Instance.UpdatedMoney >= Mathf.Abs(lawCost) && TownHallUIManager.Instance.ActiveLaw == LawOptions.NoLawActive)
            {
                enactLawButton.interactable = true;
            }
            else
            {
                enactLawButton.interactable = false;
            }

            if(TownHallUIManager.Instance.ActiveLaw == LawOption)
            {
                revokeLawButton.interactable = true;
            }
            else
            {
                revokeLawButton.interactable = false;
            }
        }


        protected virtual void OpenDescription()
        {
            TownHallUIManager.Instance.CloseDescriptions();
            if (!lawDescription.activeInHierarchy) lawDescription.SetActive(true);
        }


        public void CloseDescription()
        {
            if(lawDescription.activeInHierarchy) lawDescription.SetActive(false);
        }


        public void EnactLaw()
        {
            EconomyManager.Instance.UpdateMoney(lawCost);
            ClockUI.Instance.StartCoroutine(ClockUI.Instance.SetLawStartTime(ClockUI.Instance.day));
            PopulationManager.Instance.PopulationComplyingToLaw = new List<Entity>();

            TownHallUIManager.Instance.LawEnacted(this);

            if (LawOption == LawOptions.Lockdown)
            {
                TownHallUIManager.Instance.ActiveLaw = LawOptions.Lockdown;

                foreach (Entity person in PopulationManager.Instance.Population)
                {
                    float prob = UnityEngine.Random.value;
                    if (prob <= person.Feelings.ApprovalRating)
                    {
                        person.ChangeBehaviorTree(Entity.BehaviourTreeType.Lockdown);
                        PopulationManager.Instance.AddPersonComplyingToLaw(person);
                    }
                }
            }
            else if (LawOption == LawOptions.RemoteWork)
            {
                TownHallUIManager.Instance.ActiveLaw = LawOptions.RemoteWork;

                foreach (Entity person in PopulationManager.Instance.Population)
                {
                    float prob = UnityEngine.Random.value;
                    if (prob <= person.Feelings.ApprovalRating)
                    {
                        person.ChangeBehaviorTree(Entity.BehaviourTreeType.RemoteWork);
                        PopulationManager.Instance.AddPersonComplyingToLaw(person);
                    }
                }
            }
            else if (LawOption == LawOptions.ForcedLabour)
            {
                TownHallUIManager.Instance.ActiveLaw = LawOptions.ForcedLabour;

                foreach (Entity person in PopulationManager.Instance.Population)
                {
                    float prob = UnityEngine.Random.value;
                    if (prob <= person.Feelings.ApprovalRating && person.currentAction != Entity.CurrentAction.GoingToTheHospital && person.currentAction != Entity.CurrentAction.GettingCured)
                    {
                        person.ChangeBehaviorTree(Entity.BehaviourTreeType.ForcedLabour);
                        PopulationManager.Instance.AddPersonComplyingToLaw(person);
                    }
                }
            }

            TownHallUIManager.Instance.ActiveLawGO.gameObject.SetActive(true);
            TownHallUIManager.Instance.ActiveLawGO.GetComponent<RawImage>().color = new Color(1, 1, 1, 0.25f);

            TownHallUIManager.Instance.ActiveLawGO.RegisterLaw(this);
            TownHallUIManager.Instance.ActiveLawGO.ChangeText();
        }

        public void RevokeLaw()
        {
            PopulationManager.Instance.PopulationComplyingToLaw = new List<Entity>();
            TownHallUIManager.Instance.ActiveLaw = LawOptions.NoLawActive;
            ClockUI.Instance.StopCoroutine("SetLawStartTime");


            TownHallUIManager.Instance.ActiveLawGO.gameObject.SetActive(false);

            foreach (Entity person in PopulationManager.Instance.Population)
            {
                person.StartTimeOfLaw = -1;
                person.TestedTimeComplyingToLaw = false;
                person.DaysOfComplianceToLaw = 0;
                person.ChangeBehaviorTree(Entity.BehaviourTreeType.NormalRoutine);
                person.Feelings.UpdateHappiness(person.Workshift);
            }

        }
    }
}
