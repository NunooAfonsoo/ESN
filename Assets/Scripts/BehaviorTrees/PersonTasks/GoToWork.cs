using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Buildings;
using Assets.Scripts.Citizens;

namespace Assets.Scripts.BehaviourTrees.PersonTasks
{
    public class GoToWork : Task
    {
        Entity person;

        Workplace workplace;

        Transform workplaceDoor;

        public GoToWork(Entity person, Workplace workplace, Transform workplaceDoor)
        {
            this.person = person;
            this.workplace = workplace;
            this.workplaceDoor = workplaceDoor;
        }

        public override Result Run()
        {
            if(workplace == null || workplaceDoor == null || person.GetCurrentTime() != Entity.CurrentTime.WorkTime || person.currentAction == Entity.CurrentAction.Working
                || person.currentAction == Entity.CurrentAction.GettingCured || person.currentAction == Entity.CurrentAction.GoingToTheHospital)
            {
                return Result.Failure;
            }

            if(person.Arrived(workplaceDoor.position) && person.currentAction == Entity.CurrentAction.GoingToWork && person.GetCurrentTime() == Entity.CurrentTime.WorkTime)
            {
                if(person.GoInsideBuilding(workplace))
                {
                    person.currentAction = Entity.CurrentAction.Working;
                    return Result.Success;
                }
                return Result.Failure;
            }
            else if(person.currentAction == Entity.CurrentAction.GoingToWork && Vector3.Distance(person.navMeshAgent.destination, workplaceDoor.position) <= 0.5f)
            {
                return Result.Success;
            }
            else
            {
                if(person.navMeshAgent.destination != workplaceDoor.position) person.MoveTo(workplaceDoor.position);
                if(person.currentAction != Entity.CurrentAction.GoingToWork) person.currentAction = Entity.CurrentAction.GoingToWork;
                return Result.Success;
            }
        }
    }
}
