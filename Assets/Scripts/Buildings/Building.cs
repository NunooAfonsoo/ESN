using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Citizens;

namespace Assets.Scripts.Buildings
{
    public abstract class Building : MonoBehaviour
    {
        public List<Transform> doorsTransform;

        public List<Entity> peopleInside;

        [SerializeField] private bool drawRadius;
        [SerializeField] private float radius;

        protected virtual void Awake()
        {
            doorsTransform = new List<Transform>();
            peopleInside = new List<Entity>();
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

        }

        public virtual bool TryEnterBuilding(Entity person, Hospital.HospitalUnit unit)
        {
            peopleInside.Add(person);
            return true;
        }


        public virtual void LeaveBuilding(Entity person, Hospital.HospitalUnit unit)
        {
            peopleInside.Remove(person);
        }


        private void OnDrawGizmos()
        {
            if(drawRadius)
            {
                Gizmos.color = Color.red;
                float theta = 0;
                float x = 0.2f * Mathf.Cos(theta);
                float y = 0.2f * Mathf.Sin(theta);
                Vector3 pos = doorsTransform[0].position + new Vector3(x, 0, y);
                Vector3 newPos = pos;
                Vector3 lastPos = pos;
                for (theta = 0.02f; theta < Mathf.PI * 2; theta += 0.02f)
                {
                    x = 0.2f * Mathf.Cos(theta);
                    y = 0.2f * Mathf.Sin(theta);
                    newPos = doorsTransform[0].position + new Vector3(x, 0, y);
                    Gizmos.DrawLine(pos, newPos);
                    pos = newPos;
                }
                Gizmos.DrawLine(pos, lastPos);

                Gizmos.color = Color.black;
                float theta1 = 0;
                float x1 = radius * Mathf.Cos(theta1);
                float y1 = radius * Mathf.Sin(theta1);
                Vector3 pos1 = doorsTransform[0].position + new Vector3(x1, 0, y1);
                Vector3 newPos1 = pos1;
                Vector3 lastPos1 = pos1;
                for (theta1 = 0.2f; theta1 < Mathf.PI * 2; theta1 += 0.002f)
                {
                    x1 = radius * Mathf.Cos(theta1);
                    y1 = radius * Mathf.Sin(theta1);
                    newPos1 = doorsTransform[0].position + new Vector3(x1, 0, y1);
                    Gizmos.DrawLine(pos1, newPos1);
                    pos1 = newPos1;
                }
                Gizmos.DrawLine(pos1, lastPos1);
            }

            //Gizmos.DrawLine(transform.position, transform.position + new Vector3(70 ,0 , 0));

        }


    } 
}
