using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI.LabUI
{
    public class CureTechTree : TechTree
    {
        public static CureTechTree Instance;

        protected override void Awake()
        {
            Instance = this;
        }
    }
}