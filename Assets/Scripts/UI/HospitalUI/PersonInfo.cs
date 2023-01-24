using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Buildings;
using Assets.Scripts.Citizens;


namespace Assets.Scripts.UI.HospitalUI
{
    public class PersonInfo : MonoBehaviour
    {
        private Button sendToOtherUnit;
        private Button sendHome;
        public Hospital.HospitalUnit Unit;
        public Entity Person;
        private RawImage diseaseState;

        public Action<PersonInfo, Hospital.HospitalUnit> OnUnitChange;


        private void Awake()
        {
            diseaseState = transform.GetChild(0).GetComponent<RawImage>();
            sendToOtherUnit = transform.GetChild(1).GetChild(0).GetComponent<Button>();
            sendHome = transform.GetChild(1).GetChild(1).GetComponent<Button>();

            sendToOtherUnit.onClick.AddListener(delegate { SendToOtherUnit(); });
            sendHome.onClick.AddListener(delegate { SendHome(); });

        }


        private void Update()
        {
            SetButtonInteractability();

            diseaseState.color = new Color(1 - Person.Health / 100, Person.Health / 100, 0);
        }

        private void SetButtonInteractability()
        {
            switch (Unit)
            {
                case Hospital.HospitalUnit.NormalUnit:
                    if (Hospital.Instance.GetCurrentICUOccupation() + 1 > Hospital.Instance.GetICUCapacity())
                        sendToOtherUnit.interactable = false;
                    else sendToOtherUnit.interactable = true;
                    break;
                case Hospital.HospitalUnit.IntensiveCareUnit:
                    if (Hospital.Instance.GetCurrentNUOccupation() + 1 > Hospital.Instance.GetNormalUnitCapacity())
                        sendToOtherUnit.interactable = false;
                    else sendToOtherUnit.interactable = true;
                    break;
            }
        }


        public void SendToOtherUnit()
        {
            switch(Unit)
            {
                case Hospital.HospitalUnit.NormalUnit:
                    OnUnitChange?.Invoke(this, Hospital.HospitalUnit.IntensiveCareUnit);
                    break;
                case Hospital.HospitalUnit.IntensiveCareUnit:
                    OnUnitChange?.Invoke(this, Hospital.HospitalUnit.NormalUnit);
                    break;
            }
        }

        private void SendHome()
        {
            if (Person.HealCoroutine != null)
            {
                Person.StopCoroutine(Person.HealCoroutine);
                Person.HealCoroutine = null;
            }
            Person.StartDiseaseTicks();
            Person.TriedGoingToTheHospital = true;
            Person.InvokeRepeating("ResetTriedGoingToTheHospital", 10f, 1f);
            Hospital.Instance.LeaveBuilding(Person, Unit);
        }
    }
}
