using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Buildings;
using Assets.Scripts.Citizens;

namespace Assets.Scripts.BehaviourTrees.PersonTasks
{
    public class GoToHome : Task
    {
        Entity person;

        Home home;

        Transform homeDoor;

        public GoToHome(Entity person, Home home, Transform homeDoor)
        {
            this.person = person;
            this.home = home;
            this.homeDoor = homeDoor;
        }

        public override Result Run()
        {
            if(home == null || homeDoor == null|| person.GetCurrentTime() != Entity.CurrentTime.RestTime || person.currentAction == Entity.CurrentAction.Resting
                || person.currentAction == Entity.CurrentAction.GettingCured || person.currentAction == Entity.CurrentAction.GoingToTheHospital)
            {
                return Result.Failure;
            }

            if(person.Arrived(homeDoor.position) && person.currentAction == Entity.CurrentAction.GoingHome && person.GetCurrentTime() == Entity.CurrentTime.RestTime)
            {
                if(person.GoInsideBuilding(home))
                {
                    person.currentAction = Entity.CurrentAction.Resting;
                    return Result.Success;
                }
                return Result.Failure;
            }
            else if(person.currentAction == Entity.CurrentAction.GoingHome && Vector3.Distance(person.navMeshAgent.destination, homeDoor.position) <= 0.5f)
            {
                return Result.Success;
            }
            else
            {
                if(person.navMeshAgent.destination != homeDoor.position) person.MoveTo(homeDoor.position);
                if(person.currentAction != Entity.CurrentAction.GoingHome) person.currentAction = Entity.CurrentAction.GoingHome;
                return Result.Running;
            }
        }
    }
}