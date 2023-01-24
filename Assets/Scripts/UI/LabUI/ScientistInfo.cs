using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Citizens;
using Assets.Scripts.Managers;

using System;

namespace Assets.Scripts.UI.LabUI
{
    public class ScientistInfo : MonoBehaviour
    {

        private Button FireScientistButton;
        public Scientist Scientist;

        public event Action<Scientist> OnScientistFired;

        private void Awake()
        {
            FireScientistButton = transform.GetChild(0).GetChild(0).GetComponent<Button>();

            FireScientistButton.onClick.AddListener(delegate { FireScientist(); });
        }


        private void FireScientist()
        {
            PopulationManager.Instance.RemoveScientist(Scientist);
            OnScientistFired?.Invoke(Scientist);
            LabUIManager.Instance.FireScientist(this);
        }
    }
}
