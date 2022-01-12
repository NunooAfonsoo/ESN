using System;
using UnityEngine;
using UnityEngine.AI;
using Assets.Scripts.Managers;
using Assets.Scripts.Buildings;
using System.Collections.Generic;
using Assets.Scripts.BehaviourTrees;
using Assets.Scripts.Disease;
using Assets.Scripts.UI.HospitalUI;



namespace Assets.Scripts.Citizens
{
    public class Scientist : Entity
    {
        private float experienceInterval;
        private float lastExperienceTime;

        [SerializeField] private int lowerExperienceBoundary;
        [SerializeField] private int higherExperienceBoundary;

        public new event Action<Entity, bool> OnSpawn;
        public new event Action<Entity, bool> OnDeath;

        protected override void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            capsuleCollider = GetComponent<CapsuleCollider>();
            isScientist = true;

            PopulationManager.Instance.RegisterScientist(this);
            OnSpawn?.Invoke(this, false);


            /* Getting Workplace location */
            workplace = GameObject.Find("Lab").GetComponent<Workplace>();
            workplaceDoor = workplace.doorsTransform[0];

            /* Getting Home location */
            ChooseHome();

            /* Getting FTP locations */
            ChooseFreeTimePlaces();


            Health = 100f;
            if(PatientZero) SetInfection(new Coronga());
            else Disease = null;
            hospitalUnit = Hospital.HospitalUnit.NotInHospital;


            navMeshAgent?.SetDestination(Vector3.zero);
            if(navMeshAgent) navMeshAgent.isStopped = true;
            ChangeBehaviorTree(BehaviourTreeType.NormalRoutine);

            lastWorkTime = 0.0f;
            workInterval = 5.0f;
            Workshift = 0.5f;

            startWorkTime = UnityEngine.Random.Range(0.500f, 0.733f);  //between 6 and 10
            startFTPTime = startWorkTime + Workshift;      //between 12 and 16
            startHomeTime = UnityEngine.Random.Range(1.500f, 1.833f);  //between 18 and 22


            float startingApprovalRating = UnityEngine.Random.Range(0.35f, 0.9f);
            float startinghappiness = UnityEngine.Random.Range((Workshift + startingApprovalRating) / 2 - 0.15f, (Workshift + startingApprovalRating) / 2 + 0.15f);
            float startingProductivity = 0.6f * startinghappiness + 0.4f * Workshift;
            float startingDiseaseFear = 0;
            Feelings = new Feelings(startinghappiness, startingProductivity, startingDiseaseFear, startingApprovalRating);



            experienceInterval = 0f;
            experienceInterval = 5f;
        }


        protected override void FixedUpdate()
        {
            if(CurrentBuilding && CurrentBuilding.GetComponent<Lab>())
            {
                lastExperienceTime += Time.deltaTime;
                if(lastExperienceTime > experienceInterval)
                { 
                    lastExperienceTime = 0f;
                    PerformExperience();
                }
            }
        }


        internal void GetFired()
        {
            PopulationManager.Instance.RemoveScientist(this);
            if (Disease != null)
            {
                PopulationManager.Instance.RemoveFromInfected(this);
                StopDiseaseTicks();

            }


            if (hospitalUnit != Hospital.HospitalUnit.NotInHospital) HospitalUIManager.Instance.RemovePersonInfo(this, hospitalUnit);

            Destroy(gameObject);
        }

        protected override void Die()
        {
            PopulationManager.Instance.RemoveScientist(this);
            PopulationManager.Instance.RemoveFromInfected(this);

            PopulationManager.Instance.Deaths++;

            StopDiseaseTicks();

            OnDeath?.Invoke(this, true);

            if(hospitalUnit != Hospital.HospitalUnit.NotInHospital) HospitalUIManager.Instance.RemovePersonInfo(this, hospitalUnit);

            Destroy(gameObject);
        }
        public override void MoveTo(Vector3 destination)
        {
            if(navMeshAgent)
            {
                if(!navMeshAgent.enabled)
                {
                    navMeshAgent.enabled = true;
                    navMeshAgent.isStopped = false;
                    navMeshAgent.SetDestination(destination);

                    capsuleCollider.enabled = true;


                    if(CurrentBuilding.GetComponent<Lab>())
                    {
                        Lab lab = (Lab)CurrentBuilding;
                        lab.LeaveLab(this);
                    }
                    else
                    {
                        CurrentBuilding.LeaveBuilding(this, Hospital.HospitalUnit.NotInHospital);
                    }

                    CurrentBuilding = null;
                    EnableMeshRenderer();
                }
                else
                {
                    navMeshAgent.isStopped = false;
                    navMeshAgent.SetDestination(destination);
                }
            }
        }


        public override bool GoInsideBuilding(Building buildingToEnter, Hospital.HospitalUnit unit = Hospital.HospitalUnit.NotInHospital)
        {
            Lab lab = buildingToEnter.GetComponent<Lab>();

            if(lab)
            {
                navMeshAgent.enabled = false;
                capsuleCollider.enabled = false;
                lab.EnterLab(this);
                CurrentBuilding = lab;
                DisableMeshRenderer();
                return true;
            }
            else
            {
                if(buildingToEnter.TryEnterBuilding(this, unit))
                {

                    CurrentBuilding = buildingToEnter;
                    navMeshAgent.enabled = false;
                    capsuleCollider.enabled = false;
                    DisableMeshRenderer();
                    SetHospitalUnit(unit);
                    return true;
                }

                TriedGoingToTheHospital = true;
                return false;
            }

        }


        public void PerformExperience()
        {
            int experienceOutcome = UnityEngine.Random.Range(lowerExperienceBoundary, higherExperienceBoundary);
            EconomyManager.Instance.UpdateScience(experienceOutcome);
        }
    }
}
