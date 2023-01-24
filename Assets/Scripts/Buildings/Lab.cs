using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Citizens;
using Assets.Scripts.Managers;


namespace Assets.Scripts.Buildings
{
    public class Lab : Workplace
    {
        public static Lab Instance;

        [SerializeField] private GameObject labMenuObject;

        List<Scientist> scientistsInside;

        private float labUpkeep;

        protected override void Awake()
        {
            base.Awake();
            scientistsInside = new List<Scientist>();
            Instance = this;

            UpdateUpkeep();
        }


        private void OnMouseDown()
        {
            if (UIManager.Instance.ActiveMenu == null && !GameManager.Instance.PauseMenuActive)
            {
                labMenuObject.SetActive(true);
                UIManager.Instance.ActiveMenu = labMenuObject;
            }
        }

        void UpdateUpkeep()
        {
            labUpkeep = -(10 + PopulationManager.Instance.Scientists.Count * 1.2f);
            EconomyManager.Instance.UpdateLabUpkeep(labUpkeep);
        }
        public void EnterLab(Scientist scientist)
        {
            scientistsInside.Add(scientist);
        }


        public void LeaveLab(Scientist scientist)
        {
            scientistsInside.Remove(scientist);
        }


    }
}