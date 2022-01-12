using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Citizens;

namespace Assets.Scripts.BehaviourTrees.PersonTasks
{
    public class LeisureTime : Task
    {
        Entity person;

        public LeisureTime(Entity person)
        {
            this.person = person;
        }


        public override Result Run()
        {
            if(person.GetCurrentTime() != Entity.CurrentTime.LeisureTime)
            {
                return Result.Failure;
            }

            if(person.currentAction == Entity.CurrentAction.Leisuring)
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
