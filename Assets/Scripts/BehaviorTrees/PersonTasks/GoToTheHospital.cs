using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Buildings;
using Assets.Scripts.Citizens;


namespace Assets.Scripts.BehaviourTrees.PersonTasks
{
    public class GoToTheHospital : Task
    {
        Entity person;

        Hospital hospital;

        Transform hospitalDoor;

        public GoToTheHospital(Entity person, Hospital hospital, Transform hospitalDoor)
        {
            this.person = person;
            this.hospital = hospital;
            this.hospitalDoor = hospitalDoor;
        }

        public override Result Run()
        {
            if(hospital == null || hospitalDoor == null || !person.NeedsHospitalCare() || person.currentAction == Entity.CurrentAction.GettingCured || person.TriedGoingToTheHospital)
            {
                if(person.currentAction == Entity.CurrentAction.GoingToTheHospital) person.currentAction = Entity.CurrentAction.GoingHome;
                return Result.Failure;
            }

            if(person.Arrived(hospitalDoor.position))
            {
                if (person.GoInsideBuilding(hospital, Hospital.HospitalUnit.NormalUnit))
                {
                    person.currentAction = Entity.CurrentAction.GettingCured;
                    return Result.Success;
                }
                else if (person.GoInsideBuilding(hospital, Hospital.HospitalUnit.IntensiveCareUnit))
                {
                    person.currentAction = Entity.CurrentAction.GettingCured;
                    return Result.Success;
                }
                else
                {
                    person.TriedGoingToTheHospital = true;
                    person.InvokeRepeating("ResetTriedGoingToTheHospital", 10f, 1f);
                    return Result.Failure;
                }
            }
            else if(person.currentAction == Entity.CurrentAction.GoingToTheHospital && Vector3.Distance(person.navMeshAgent.destination, hospitalDoor.position) <= 0.5f)
            {
                return Result.Success;
            }
            else
            {
                float probability = 20f + ((100f - person.Health) - 60f) / 10f * (90f - 10f);

                // if person.health == 30, prob = 100,,, prob / 10000 = 1.0% chance every BehaviorTree.Run()
                // if person.health == 20, prob = 180,,, prob / 10000 = 1.8% chance every BehaviorTree.Run()
                // if person.health == 10, prob = 260,,, prob / 10000 = 2.6% chance every BehaviorTree.Run()

                if(Random.value < (probability / 10000))
                {
                    if(person.navMeshAgent.destination != hospitalDoor.position) person.MoveTo(hospitalDoor.position);
                    if(person.currentAction != Entity.CurrentAction.GoingToTheHospital) person.currentAction = Entity.CurrentAction.GoingToTheHospital;

                    return Result.Success;
                }
                return Result.Failure;
            }
        }
    }
}
