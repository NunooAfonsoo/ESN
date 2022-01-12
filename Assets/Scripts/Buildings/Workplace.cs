using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Managers;

namespace Assets.Scripts.Buildings
{
    public class Workplace : Building
    {
        protected override void Awake()
        {
            base.Awake();
            if(gameObject.name != "Lab")
            {
                BuildingsManager.Instance.RegisterWorkPlace(transform);
            }
        }
    }
}