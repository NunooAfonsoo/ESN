using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Citizens;

namespace Assets.Scripts.BehaviourTrees.PersonTasks
{
    public class Working : Task
    {
        Entity person;

        public Working(Entity person)
        {
            this.person = person;
        }


        public override Result Run()
        {
            if(person.GetCurrentTime() != Entity.CurrentTime.WorkTime)
            {
                return Result.Failure;
            }

            if(person.currentAction == Entity.CurrentAction.Working)
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