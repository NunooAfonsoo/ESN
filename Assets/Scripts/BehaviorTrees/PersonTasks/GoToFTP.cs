using UnityEngine;
using Assets.Scripts.Buildings;
using Assets.Scripts.Citizens;

namespace Assets.Scripts.BehaviourTrees.PersonTasks
{
    public class GoToFTP : Task
    {
        Entity person;

        FreeTimePlace freeTimePlace;

        Transform freeTimePlaceDoor;

        public GoToFTP(Entity person, FreeTimePlace freeTimePlace, Transform freeTimePlaceDoor)
        {
            this.person = person;
            this.freeTimePlace = freeTimePlace;
            this.freeTimePlaceDoor = freeTimePlaceDoor;
        }

        public override Result Run()
        {
            if(freeTimePlace == null || freeTimePlaceDoor == null || person.GetCurrentTime() != Entity.CurrentTime.LeisureTime || person.currentAction == Entity.CurrentAction.Leisuring 
                || person.currentAction == Entity.CurrentAction.GettingCured || person.currentAction == Entity.CurrentAction.GoingToTheHospital)
            {
                return Result.Failure;
            }

            if(person.Arrived(freeTimePlaceDoor.position) && person.currentAction == Entity.CurrentAction.GoingToFTP && person.GetCurrentTime() == Entity.CurrentTime.LeisureTime)
            {
                if(person.GoInsideBuilding(freeTimePlace))
                {
                    person.currentAction = Entity.CurrentAction.Leisuring;
                    return Result.Success;
                }
                return Result.Failure;
            }
            else if(person.currentAction == Entity.CurrentAction.GoingToFTP && Vector3.Distance(person.navMeshAgent.destination, freeTimePlaceDoor.position) <= 0.5f)
            {
                return Result.Success;
            }
            else
            {
                if(person.navMeshAgent.destination != freeTimePlaceDoor.position) person.MoveTo(freeTimePlaceDoor.position);
                if(person.currentAction != Entity.CurrentAction.GoingToFTP) person.currentAction = Entity.CurrentAction.GoingToFTP;
                return Result.Success;
            }
        }
    }
}
