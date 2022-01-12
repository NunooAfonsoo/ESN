using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts.UI.LabUI
{
    public class TreatmentTechTree : TechTree
    {
        public static TreatmentTechTree Instance;

        protected override void Awake()
        {
            Instance = this;
        }
    }
}
