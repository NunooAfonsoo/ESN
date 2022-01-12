using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Managers;

namespace Assets.Scripts.Buildings
{
    public class FreeTimePlace : Building
    {
        protected override void Awake()
        {
            base.Awake();
            BuildingsManager.Instance.RegisterFTP(transform);
        }
    }
}