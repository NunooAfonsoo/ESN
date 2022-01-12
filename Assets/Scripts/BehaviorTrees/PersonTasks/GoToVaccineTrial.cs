using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Buildings;
using Assets.Scripts.Citizens;

namespace Assets.Scripts.BehaviourTrees.PersonTasks
{
    public class GoToVaccineTrial : Task
    {
        Entity person;

        Lab lab;

        Transform labDoor;

        public GoToVaccineTrial(Entity person, Lab lab, Transform labDoor)
        {
            this.person = person;
            this.lab = lab;
            this.labDoor = labDoor;
        }

        public override Result Run()
        {
            if (lab == null || labDoor == null || !person.TrialsOnHumans)
                return Result.Failure;

            if (person.Arrived(labDoor.position))
            {
                if (person.GoInsideBuilding(lab))
                {
                    person.currentAction = Entity.CurrentAction.DoingVaccineTrial;
                    int waitTime = Random.Range(5, 40);
                    person.InvokeRepeating("ResetTrialsOnHumans", waitTime, 1f); 
                    return Result.Success;
                }
                return Result.Failure;
            }
            else if (person.currentAction == Entity.CurrentAction.GoingToVaccineTrial && Vector3.Distance(person.navMeshAgent.destination, labDoor.position) <= 0.5f)
            {
                return Result.Success;
            }

            else if (person.TrialsOnHumans)
            {
                if (person.navMeshAgent.destination != labDoor.position) person.MoveTo(labDoor.position);
                person.currentAction = Entity.CurrentAction.GoingToVaccineTrial;
                return Result.Success;
            }
            else return Result.Failure;
        }
    }
}
