using Assets.Scripts.Citizens;
using UnityEngine;

namespace Assets.Scripts.BehaviourTrees.PersonTasks
{
    public class RestingAtHome : Task
    {

        Entity person;

        public RestingAtHome(Entity person)
        {
            this.person = person;
        }


        public override Result Run()
        {
            if(person.GetCurrentTime() != Entity.CurrentTime.RestTime)
            {
                return Result.Failure;
            }

            if(person.currentAction == Entity.CurrentAction.Resting)
            {
                return Result.Success;
            }
            else
            {
                return Result.Failure;
            }
        }
    }

}