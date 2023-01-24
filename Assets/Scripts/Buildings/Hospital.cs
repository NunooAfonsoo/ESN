using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Managers;
using Assets.Scripts.Citizens;
using Assets.Scripts.UI.HospitalUI;
using System;
using UnityEngine.UI;

namespace Assets.Scripts.Buildings
{
    public class Hospital : Building
    {
        public static Hospital Instance;

        [SerializeField] private GameObject hospitalMenuObject;

        [SerializeField] private int NormalUnitCap;
        [SerializeField] private int ICUCap;

        private int currentNUOccupation;
        private int currentICUOccupation;

        public List<Entity> peopleInNU;
        public List<Entity> peopleInICU;

        public static event Action<HospitalUnit> OnNormalUnitEntry;
        public static event Action<HospitalUnit> OnICUEntry;
        public static event Action<HospitalUnit> OnHospitalExit;

        private float NUStepEvolution;
        private float ICUStepEvolution;
        private float NUTimeStepEvolution;
        private float ICUTimeStepEvolution;

        private float probOfTreatmentSuccess;

        public enum HospitalUnit
        {
            NotInHospital,
            NormalUnit,
            IntensiveCareUnit
        }


        private float hospitalUpkeep;

        protected override void Awake()
        {
            base.Awake();
            Instance = this;
            currentNUOccupation = 0;
            currentICUOccupation = 0;


            doorsTransform = new List<Transform>();



            for (int i = 0; i < transform.childCount; i++)
            {
                if(transform.GetChild(i).name == "Door")
                {
                    doorsTransform.Add(transform.GetChild(i));
                }
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                if(transform.GetChild(i).name.Contains("Cube"))
                {
                    transform.GetChild(i).GetComponent<MeshRenderer>().enabled = false;
                }
            }

            NUStepEvolution = 0.5f;
            ICUStepEvolution = 1f;
            NUTimeStepEvolution = 0.5f;
            ICUTimeStepEvolution = 0.5f;

            probOfTreatmentSuccess = 0.60f;

            UpdateUpkeep();
        }

        private void OnMouseDown()
        {
            if (UIManager.Instance.ActiveMenu == null && !GameManager.Instance.PauseMenuActive)
            {
                UIManager.Instance.ActiveMenu = hospitalMenuObject;
                hospitalMenuObject.SetActive(true);
            }
        }

        public override bool TryEnterBuilding(Entity person, HospitalUnit unit)
        {
            switch(unit)
            {
                case HospitalUnit.NormalUnit:
                    if(currentNUOccupation + 1 <= NormalUnitCap)
                    {
                        person.StopDiseaseTicks();
                        peopleInNU.Add(person);
                        HospitalUIManager.Instance.CreateNewPersonInfo(person, unit);
                        OnNormalUnitEntry?.Invoke(unit);
                        if (person.HealCoroutine != null)
                        {
                            person.StopCoroutine(person.HealCoroutine);
                            person.HealCoroutine = null;
                        }
                        person.HealCoroutine = person.StartCoroutine(person.Heal(NUStepEvolution, NUTimeStepEvolution, probOfTreatmentSuccess));
                        UpdateUpkeep();
                        return true;
                    }
                    return false;
                case HospitalUnit.IntensiveCareUnit:
                    if(currentICUOccupation + 1 <= ICUCap)
                    {
                        person.StopDiseaseTicks();
                        peopleInICU.Add(person);
                        HospitalUIManager.Instance.CreateNewPersonInfo(person, unit);
                        OnICUEntry?.Invoke(unit);
                        if (person.HealCoroutine != null)
                        {
                            person.StopCoroutine(person.HealCoroutine);
                            person.HealCoroutine = null;
                        }
                        person.HealCoroutine = person.StartCoroutine(person.Heal(ICUStepEvolution, ICUTimeStepEvolution, probOfTreatmentSuccess));
                        UpdateUpkeep();
                        return true;
                    }
                    return false;
            }
            return false;
        }


        void UpdateUpkeep()
        {
            hospitalUpkeep = -(1.5f * GetNormalUnitCapacity() + 0.5f * GetCurrentNUOccupation() + 3.0f * GetICUCapacity() + 1.0f * GetCurrentICUOccupation());
            EconomyManager.Instance.UpdateHospitalUpkeep(hospitalUpkeep);
        }

        public void UpgradeNU(float upgradeAmount)
        {
            foreach(Entity person in peopleInNU)
            {
                person.StopCoroutine(person.HealCoroutine);
                person.HealCoroutine = null;
            }
            NUStepEvolution *= upgradeAmount;
            foreach (Entity person in peopleInNU)
            {
                person.HealCoroutine = person.StartCoroutine(person.Heal(NUStepEvolution, NUTimeStepEvolution, probOfTreatmentSuccess));
            }
        }


        public void UpgradeICU(float upgradeAmount)
        {
            foreach (Entity person in peopleInICU)
            {
                person.StopCoroutine(person.HealCoroutine);
                person.HealCoroutine = null;
            }
            ICUStepEvolution *= upgradeAmount;
            foreach (Entity person in peopleInICU)
            {
                person.HealCoroutine = person.StartCoroutine(person.Heal(ICUStepEvolution, ICUTimeStepEvolution, probOfTreatmentSuccess));
            }
        }

        public void UpgradeNUTimeStep(float upgradeAmount)
        {
            foreach (Entity person in peopleInNU)
            {
                person.StopCoroutine(person.HealCoroutine);
                person.HealCoroutine = null;
            }
            NUTimeStepEvolution *= upgradeAmount;
            foreach (Entity person in peopleInNU)
            {
                person.HealCoroutine = person.StartCoroutine(person.Heal(NUStepEvolution, NUTimeStepEvolution, probOfTreatmentSuccess));
            }
        }


        public void UpgradeICUTimeStep(float upgradeAmount)
        {
            foreach (Entity person in peopleInICU)
            {
                person.StopCoroutine(person.HealCoroutine);
                person.HealCoroutine = null;
            }
            ICUTimeStepEvolution *= upgradeAmount;
            foreach (Entity person in peopleInICU)
            {
                person.HealCoroutine = person.StartCoroutine(person.Heal(ICUStepEvolution, ICUTimeStepEvolution, probOfTreatmentSuccess));
            }
        }


        public void ImproveProbOfTreatmentSuccess(float improvement)
        {
            probOfTreatmentSuccess *= improvement;

            foreach (Entity person in peopleInNU)
            {
                person.StopCoroutine(person.HealCoroutine);
                person.HealCoroutine = null;
                person.HealCoroutine = person.StartCoroutine(person.Heal(NUStepEvolution, NUTimeStepEvolution, probOfTreatmentSuccess));
            }

            foreach (Entity person in peopleInICU)
            {
                person.StopCoroutine(person.HealCoroutine);
                person.HealCoroutine = null;
                person.HealCoroutine = person.StartCoroutine(person.Heal(ICUStepEvolution, ICUTimeStepEvolution, probOfTreatmentSuccess));
            }

        }

        public override void LeaveBuilding(Entity person, Hospital.HospitalUnit unit)
        {
            switch (unit)
            {
                case Hospital.HospitalUnit.NormalUnit:
                    peopleInNU.Remove(person);
                    HospitalUIManager.Instance.RemovePersonInfo(person, unit);
                    OnHospitalExit?.Invoke(unit);
                    person.currentAction = Entity.CurrentAction.GoingHome;
                    person.SetHospitalUnit(Hospital.HospitalUnit.NotInHospital);
                    UpdateUpkeep();
                    break;
                case Hospital.HospitalUnit.IntensiveCareUnit:
                    peopleInICU.Remove(person);
                    HospitalUIManager.Instance.RemovePersonInfo(person, unit);
                    OnHospitalExit?.Invoke(unit);
                    person.currentAction = Entity.CurrentAction.GoingHome;
                    person.SetHospitalUnit(Hospital.HospitalUnit.NotInHospital);
                    UpdateUpkeep();
                    break;
            }
        }



        public int GetNormalUnitCapacity()
        {
            return NormalUnitCap;
        }

        public int GetICUCapacity()
        {
            return ICUCap;
        }

        public void ChangeNormalUnitCapacity(int changeAmount)
        {
            NormalUnitCap += changeAmount;
            UpdateUpkeep();
        }

        public void ChangeICUCapacity(int changeAmount)
        {
            ICUCap += changeAmount;
            UpdateUpkeep();
        }


        public int GetCurrentNUOccupation()
        {
            return currentNUOccupation;
        }

        public int GetCurrentICUOccupation()
        {
            return currentICUOccupation;
        }

        public void ChangeNormalUnitOccupation(int occupationChange)
        {
            currentNUOccupation += occupationChange;
        }

        public void ChangeICUOccupation(int occupationChange)
        {
            currentICUOccupation += occupationChange;
        }
    }
}
