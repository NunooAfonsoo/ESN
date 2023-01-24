using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.DayNightCycle
{
    public class MoonController : MonoBehaviour
    {
        public float Distance = 1000.0f;
        public float scale = 15.0f;


        void Start()
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, Distance);

            transform.localScale = new Vector3(scale, scale, scale);
        }

    }
}
