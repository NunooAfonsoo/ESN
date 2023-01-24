using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Assets.Scripts.Managers;
using Assets.Scripts.BehaviourTrees;
using Assets.Scripts.Buildings;
using Assets.Scripts.Clock;
using Assets.Scripts.Disease;
using System.Collections;
using Assets.Scripts.UI.HospitalUI;


namespace Assets.Scripts.Citizens
{
    public class Entity : MonoBehaviour
    {
        public NavMeshAgent navMeshAgent;
        protected CapsuleCollider capsuleCollider;
        protected bool isScientist;

        protected Task routineBehaviourTree;
        protected Home home;
        protected Transform homeDoor;

        protected Workplace workplace;
        protected Transform workplaceDoor;

        protected List<FreeTimePlace> freeTimePlaces;

        protected float startHomeTime;
        protected float startWorkTime;
        protected float startFTPTime;

        protected Dictionary<FreeTimePlace, List<Transform>> freeTimePlacesDoors;

        protected Building CurrentBuilding;


        protected float workInterval;
        protected float lastWorkTime;
        public float Workshift;
        private float lowerMoneyProduction;
        private float higherMoneyProduction;


        public enum BehaviourTreeType
        {
            NormalRoutine,
            Lockdown,
            RemoteWork,
            ForcedLabour
        }

        public BehaviourTreeType CurrentBehaviourTree;

        public float StartTimeOfLaw;
        public bool TestedTimeComplyingToLaw;
        public int DaysOfComplianceToLaw;


        public enum CurrentAction
        {
            GoingHome,
            Resting,
            GoingToWork,
            Working,
            GoingToFTP,
            Leisuring,
            GoingToTheHospital,
            GettingCured,
            DoingVaccineTrial,
            GoingToVaccineTrial,
            DoingFinalVaccine,
            GoingToFinalVaccine
        }
        public CurrentAction currentAction;

        public enum CurrentTime
        {
            RestTime,
            WorkTime,
            LeisureTime
        }
        public CurrentTime currentTime;

        public bool IsOutside
        {
            get
            {
                return currentAction == CurrentAction.GoingHome || currentAction == CurrentAction.GoingToFTP || currentAction == CurrentAction.GoingToWork;
            }
        }


        public Feelings Feelings;

        public Dictionary<string, float> DiseaseTraits { get; protected set; }
        public IDisease Disease;
        public event System.Action<Entity, bool> OnInfectedOrHealed;
        public Coroutine HealCoroutine;

        public event System.Action<Entity, bool> OnSpawn;
        public event System.Action<Entity, bool> OnDeath;

        public float Health;
        [SerializeField] protected ParticleSystem coughParticles;

        public bool TriedGoingToTheHospital;
        protected Hospital.HospitalUnit hospitalUnit;

        public bool TrialsOnHumans;
        public bool FinalVaccine;
        public bool Vaccinated;

        public bool PatientZero;

        

        protected virtual void Start()
        {


            navMeshAgent = GetComponent<NavMeshAgent>();
            capsuleCollider = GetComponent<CapsuleCollider>();
            isScientist = false;

            PopulationManager.Instance.RegisterPerson(this);
            OnSpawn?.Invoke(this, false);

            /* Getting Workplace location */
            ChooseWorkplace();

            /* Getting Home location */
            ChooseHome();

            /* Getting FTP locations */
            ChooseFreeTimePlaces();
            

            /*
            foreach (string traitName in TRAIT_KEYS)
            {
                if(traitName != "diseaseRadius") DiseaseTraits.Add(traitName, 1f);
                else DiseaseTraits.Add(traitName, 2f);
            }
            */

            Health = 100f;
            if(PatientZero) SetInfection(new Coronga());
            else Disease = null;
            TriedGoingToTheHospital = false;
            hospitalUnit = Hospital.HospitalUnit.NotInHospital;


            navMeshAgent?.SetDestination(Vector3.zero);
            if(navMeshAgent) navMeshAgent.isStopped = true;
            ChangeBehaviorTree(BehaviourTreeType.NormalRoutine);


            lastWorkTime = 0.0f;
            workInterval = 5.0f;
            Workshift = 0.5f;

            startWorkTime = Random.Range(0.500f, 0.733f);  //between 6 and 10
            startFTPTime = startWorkTime + Workshift;      //between 12 and 16
            startHomeTime = Random.Range(1.500f , 1.833f);  //between 18 and 22
            lowerMoneyProduction = 0.5f;
            higherMoneyProduction = 1.5f;

            StartTimeOfLaw = -1;
            TestedTimeComplyingToLaw = false;

            HealCoroutine = null;
            TrialsOnHumans = false;
            FinalVaccine = false;
            Vaccinated = false;


            float startingApprovalRating = Random.Range(0.35f, 0.9f);
            float startinghappiness = Random.Range((Workshift + startingApprovalRating) / 2 - 0.15f, (Workshift+ startingApprovalRating) / 2 + 0.15f);
            float startingProductivity = 0.6f * startinghappiness + 0.4f * Workshift;
            float startingDiseaseFear = 0;
            Feelings = new Feelings(startinghappiness, startingProductivity, startingDiseaseFear, startingApprovalRating);
        }

        private void Update()
        {
            routineBehaviourTree?.Run();

            if(Health < 0f)
            {
                Die();
            }

            if (StartTimeOfLaw != -1 && Mathf.Abs(ClockUI.Instance.day % 2 - StartTimeOfLaw) < 0.01f && !TestedTimeComplyingToLaw)
            {
                TestedTimeComplyingToLaw = true;
                DaysOfComplianceToLaw++;
                Invoke("ResetTestedLawCompliance", 10f);
                Feelings.UpdateHappiness(-1, GetHappinessChange());
            }
        }

        private float GetHappinessChange()
        {
            switch(CurrentBehaviourTree)
            {
                case BehaviourTreeType.Lockdown:
                    return -Feelings.Happiness / 10f * DaysOfComplianceToLaw * 2f;
                case BehaviourTreeType.RemoteWork:
                    return -Feelings.Happiness / 10f * DaysOfComplianceToLaw * 1.5f;
                case BehaviourTreeType.ForcedLabour:
                    return -Feelings.Happiness / 10f * DaysOfComplianceToLaw * 1f;
            }
            return 0;
        }

        private void ResetTestedLawCompliance()
        {
            TestedTimeComplyingToLaw = false;
        }


        protected virtual void FixedUpdate()
        {
            lastWorkTime += Time.deltaTime;
            if(!isScientist && lastWorkTime > workInterval)
            {
                lastWorkTime = 0f;
                ProduceMoney();
            }
        }


        public void ChangeBehaviorTree(BehaviourTreeType behaviourTree)
        {
            routineBehaviourTree = null;
            switch (behaviourTree)
            {
                case BehaviourTreeType.NormalRoutine:
                    lowerMoneyProduction = 0.5f;
                    higherMoneyProduction = 1.5f;
                    routineBehaviourTree = new NormalRoutineBehaviourTree(this, home, homeDoor, workplace, workplaceDoor, freeTimePlaces, freeTimePlacesDoors, Hospital.Instance, Hospital.Instance.doorsTransform[0], Lab.Instance, Lab.Instance.doorsTransform[0]);
                    break;
                case BehaviourTreeType.Lockdown:
                    StartTimeOfLaw = ClockUI.Instance.day % 2;
                    DaysOfComplianceToLaw = 0;
                    lowerMoneyProduction = 0.2f;
                    higherMoneyProduction = 1.1f;
                    routineBehaviourTree = new LockdownBehaviourTree(this, home, homeDoor, workplace, workplaceDoor, freeTimePlaces, freeTimePlacesDoors, Hospital.Instance, Hospital.Instance.doorsTransform[0], Lab.Instance, Lab.Instance.doorsTransform[0]);
                    break;
                case BehaviourTreeType.RemoteWork:
                    StartTimeOfLaw = ClockUI.Instance.day % 2;
                    DaysOfComplianceToLaw = 0;
                    lowerMoneyProduction = 0.3f;
                    higherMoneyProduction = 1.2f;
                    routineBehaviourTree = new RemoteWorkBehaviourTree(this, home, homeDoor, workplace, workplaceDoor, freeTimePlaces, freeTimePlacesDoors, Hospital.Instance, Hospital.Instance.doorsTransform[0], Lab.Instance, Lab.Instance.doorsTransform[0]);
                    break;
                case BehaviourTreeType.ForcedLabour:
                    StartTimeOfLaw = ClockUI.Instance.day % 2;
                    DaysOfComplianceToLaw = 0;
                    lowerMoneyProduction = 0.5f;
                    higherMoneyProduction = 1.5f;
                    routineBehaviourTree = new ForcedLabourBehaviourTree(this, home, homeDoor, workplace, workplaceDoor, freeTimePlaces, freeTimePlacesDoors, Hospital.Instance, Hospital.Instance.doorsTransform[0], Lab.Instance, Lab.Instance.doorsTransform[0]);
                    break;
            }
        }
        public bool NeedsHospitalCare()
        {
            return Health < 30f;
        }


        #region Disease

        protected virtual void Die()
        {
            PopulationManager.Instance.RemovePerson(this);
            PopulationManager.Instance.RemoveFromInfected(this);

            PopulationManager.Instance.Deaths++;

            StopDiseaseTicks();

            OnDeath?.Invoke(this, true);

            if(hospitalUnit != Hospital.HospitalUnit.NotInHospital) HospitalUIManager.Instance.RemovePersonInfo(this, hospitalUnit);


            Destroy(gameObject);
        }

        protected void DiseaseEvolutionTick()
        {
            if(Disease != null)
            {

                if(Random.value < 0.75f)
                {
                    Health -= 0.5f * GetTraitValue("diseaseAgressiveness");
                }
                else
                {
                    if(Health + 0.5f <= 100) Health += 0.5f;
                }
            }
        }


        protected void DiseaseSpreadTick()
        {
            if(Disease != null && IsOutside && Random.value < GetTraitValue("coughRate") * 0.5f)
            {
                Sneeze();
            }
        }



        private void Sneeze()
        {
            coughParticles.Play();
            foreach (Entity entity in PopulationManager.Instance.Population)
            {
                if(entity != this && entity.Disease == null)
                {
                    if(Vector3.Distance(entity.transform.position, transform.position) < Disease.GetHostTraitMultiplier("diseaseRadius"))
                    {
                        Debug.DrawLine(entity.transform.position, transform.position, Color.cyan);
                        entity.SetInfection(new Coronga());
                    }
                }
            }

            foreach (Entity entity in PopulationManager.Instance.Scientists)
            {
                if(entity != this && entity.Disease == null)
                {
                    if(Vector3.Distance(entity.transform.position, transform.position) < Disease.GetHostTraitMultiplier("diseaseRadius"))
                    {
                        Debug.DrawLine(entity.transform.position, transform.position, Color.cyan);
                        entity.SetInfection(new Coronga());
                    }
                }
            }
        }


        public void SetInfection(IDisease disease)
        {
            this.Disease = disease;
            PopulationManager.Instance.NewInfectedPerson(this);
            OnInfectedOrHealed?.Invoke(this, true);
            DiseaseTraits = new Dictionary<string, float>();
            StartDiseaseTicks();
        }

        public void StartDiseaseTicks()
        {
            InvokeRepeating("DiseaseEvolutionTick", 0f, 1f);
            InvokeRepeating("DiseaseSpreadTick", 0f, Disease.GetHostTraitMultiplier("diseaseRate"));
        }


        public void StopDiseaseTicks()
        {
            CancelInvoke("DiseaseEvolutionTick");
            CancelInvoke("DiseaseSpreadTick");
        }



        public float GetTraitValue(string name)
        {
            if(Disease.ContainsTrait(name))
            {
                if(Disease != null && Disease.ContainsTrait(name))
                {
                    return Disease.GetHostTraitMultiplier(name);
                }
                else
                {
                    return Disease.GetHostTraitMultiplier(name);
                }
            }
            else
            {
                return 1;
            }
        }


        public IEnumerator Heal(float healAmount, float timeStep, float probOfTreatmentSuccess)
        {
            while(Health < 100)
            {
                if (TriedGoingToTheHospital) yield return null;
                if(Random.value < probOfTreatmentSuccess) Health += healAmount;
                else Health -= healAmount / 2;

                yield return new WaitForSeconds(timeStep);
            }
            Hospital.Instance.LeaveBuilding(this, hospitalUnit);
            Disease = null;
            PopulationManager.Instance.RemoveFromInfected(this);
            Health = 100;
            OnInfectedOrHealed?.Invoke(this, false);
            yield return null;
        }

        public void StopHealCoroutine()
        {
            StopCoroutine(HealCoroutine);
            HealCoroutine = null;
        }

        #endregion

        private void ChooseWorkplace()
        {
            int workplaceIndex = Random.Range(0, BuildingsManager.Instance.Workplaces.Count);
            workplace = BuildingsManager.Instance.Workplaces[workplaceIndex].GetComponent<Workplace>();
            int workplaceDoorIndex = Random.Range(0, workplace.doorsTransform.Count);
            workplaceDoor = workplace.doorsTransform[workplaceDoorIndex];
        }

        protected void ChooseHome()
        {
            while (!home)
            {
                int homeIndex = Random.Range(0, BuildingsManager.Instance.Homes.Count);
                if(Vector3.Distance(workplaceDoor.position, BuildingsManager.Instance.Homes[homeIndex].GetComponent<Home>().doorsTransform[0].position) < 70)
                {
                    home = BuildingsManager.Instance.Homes[homeIndex].GetComponent<Home>();
                    int homeDoorIndex = Random.Range(0, home.doorsTransform.Count);
                    homeDoor = home.doorsTransform[homeDoorIndex];
                    transform.position = homeDoor.position;
                }
            }
        }

        protected void ChooseFreeTimePlaces()
        {
            freeTimePlaces = new List<FreeTimePlace>();
            freeTimePlacesDoors = new Dictionary<FreeTimePlace, List<Transform>>();

            for (int i = 0; i < BuildingsManager.Instance.FreeTimePlaces.Count; i++)
            {
                if(Vector3.Distance(workplaceDoor.position, BuildingsManager.Instance.FreeTimePlaces[i].GetComponent<FreeTimePlace>().doorsTransform[0].position) < 70)
                {
                    FreeTimePlace FTP = BuildingsManager.Instance.FreeTimePlaces[i].GetComponent<FreeTimePlace>();
                    freeTimePlaces.Add(FTP);
                    freeTimePlacesDoors.Add(FTP, FTP.doorsTransform);
                }
            }
        }

        private void ProduceMoney()
        {
            float moneyProduced = Random.Range(lowerMoneyProduction, higherMoneyProduction) * (0.7f * Workshift + 0.3f * Feelings.Productivity);
            EconomyManager.Instance.UpdateIncome(moneyProduced);
        }

        public virtual void MoveTo(Vector3 destination)
        {
            if(navMeshAgent)
            {
                if(!navMeshAgent.enabled)
                {
                    navMeshAgent.enabled = true;
                    navMeshAgent.isStopped = false;
                    navMeshAgent.SetDestination(destination);

                    capsuleCollider.enabled = true;
                    EnableMeshRenderer();

                    CurrentBuilding.LeaveBuilding(this, Hospital.HospitalUnit.NotInHospital);
                    CurrentBuilding = null;

                }
                else
                {
                    navMeshAgent.isStopped = false;
                    navMeshAgent.SetDestination(destination);
                }
            }
        }

        public void SetHospitalUnit(Hospital.HospitalUnit hospitalUnit)
        {
            this.hospitalUnit = hospitalUnit;
        }

        public CurrentTime GetCurrentTime()
        {
            float currentHour = ClockUI.Instance.day % 2;

            if (currentHour < startWorkTime || currentHour > startHomeTime)
            {
                currentTime = CurrentTime.RestTime;
                return CurrentTime.RestTime;

            }
            else if(currentHour > startWorkTime && currentHour < startFTPTime)
            {
                if(CurrentBehaviourTree == BehaviourTreeType.Lockdown || CurrentBehaviourTree == BehaviourTreeType.RemoteWork)
                {
                    currentTime = CurrentTime.RestTime;
                    return CurrentTime.RestTime;
                }
                currentTime = CurrentTime.WorkTime;
                return CurrentTime.WorkTime;
            }
            else
            {
                if (CurrentBehaviourTree == BehaviourTreeType.Lockdown)
                {
                    currentTime = CurrentTime.RestTime;
                    return CurrentTime.RestTime;
                }
                currentTime = CurrentTime.LeisureTime;
                return CurrentTime.LeisureTime;
            }
        }


        public bool Arrived(Vector3 destination)
        {
            /*
            if(navMeshAgent.enabled && navMeshAgent.destination != destination)
            {
                navMeshAgent.SetDestination(destination);
            }
            */
            return Vector3.Distance(transform.position, destination) <= 1f;
        }


        public virtual bool GoInsideBuilding(Building buildingToEnter, Hospital.HospitalUnit unit = Hospital.HospitalUnit.NotInHospital)
        {
            bool enteredBuilding = buildingToEnter.TryEnterBuilding(this, unit);
            if(enteredBuilding)
            {
                navMeshAgent.enabled = false;
                capsuleCollider.enabled = false;
                CurrentBuilding = buildingToEnter;
                DisableMeshRenderer();
                SetHospitalUnit(unit);
                return true;
            }

            return false;
        }

        void ResetTrialsOnHumans()
        {
            TrialsOnHumans = false;
            CurrentBuilding.LeaveBuilding(this, Hospital.HospitalUnit.NotInHospital);
            CancelInvoke("ResetTrialsOnHumans");
        }


        public void SetTrialsOnHumans()
        {
            TrialsOnHumans = true;
            CancelInvoke("SetTrialsOnHumans");
        }


        void SetMassVaccination()
        {
            FinalVaccine = true;
            CancelInvoke("SetMassVaccination");
        }

        void ResetMassVaccination()
        {
            FinalVaccine = false;
            CurrentBuilding.LeaveBuilding(this, Hospital.HospitalUnit.NotInHospital);
            Vaccinated = true;
            CancelInvoke("ResetMassVaccination");
        }




        void ResetTriedGoingToTheHospital()
        {
            TriedGoingToTheHospital = false;
            CancelInvoke("ResetTriedGoingToTheHospital");
        }

        protected virtual void DisableMeshRenderer()
        {
            GetComponent<Animator>().enabled = false;
            if (transform.GetChild(1).GetComponent<SkinnedMeshRenderer>())
                transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().enabled = false;
            else
            {
                
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
                
            }
        }


        protected virtual void EnableMeshRenderer()
        {
            GetComponent<Animator>().enabled = true;
            if (transform.GetChild(1).GetComponent<SkinnedMeshRenderer>())
                transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().enabled = true;
            else
            {

                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }

            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(Time.timeSinceLevelLoad > 10f)
            {
                Entity otherPerson = other.GetComponent<Entity>();
                if(Disease != null && otherPerson && otherPerson.Disease == null)
                {
                    if(IsOutside && Random.value < GetTraitValue("sweatRate") * 0.75f)
                    {
                        otherPerson.SetInfection(new Coronga());
                    }
                }
            }
        }


        public void OnDrawGizmos()
        {
            if(Disease != null)
            {
                Gizmos.color = Color.red;
                float theta = 0;
                float x = 0.2f * Mathf.Cos(theta);
                float y = 0.2f * Mathf.Sin(theta);
                Vector3 pos = transform.position + new Vector3(x, 0, y);
                Vector3 newPos = pos;
                Vector3 lastPos = pos;
                for (theta = 0.02f; theta < Mathf.PI * 2; theta += 0.02f)
                {
                    x = 0.2f * Mathf.Cos(theta);
                    y = 0.2f * Mathf.Sin(theta);
                    newPos = transform.position + new Vector3(x, 0, y);
                    Gizmos.DrawLine(pos, newPos);
                    pos = newPos;
                }
                Gizmos.DrawLine(pos, lastPos);

                Gizmos.color = Color.black;
                float theta1 = 0;
                float x1 = GetTraitValue("diseaseRadius") * Mathf.Cos(theta1);
                float y1 = GetTraitValue("diseaseRadius") * Mathf.Sin(theta1);
                Vector3 pos1 = transform.position + new Vector3(x1, 0, y1);
                Vector3 newPos1 = pos1;
                Vector3 lastPos1 = pos1;
                for (theta1 = 0.0f; theta1 < Mathf.PI * 2; theta1 += 0.1f)
                {
                    x1 = GetTraitValue("diseaseRadius") * Mathf.Cos(theta1);
                    y1 = GetTraitValue("diseaseRadius") * Mathf.Sin(theta1);
                    newPos1 = transform.position + new Vector3(x1, 0, y1);
                    Gizmos.DrawLine(pos1, newPos1);
                    pos1 = newPos1;
                }
                Gizmos.DrawLine(pos1, lastPos1);
            }
        }

    }
}