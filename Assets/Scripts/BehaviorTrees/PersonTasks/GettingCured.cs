using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Buildings;
using Assets.Scripts.Citizens;


namespace Assets.Scripts.BehaviourTrees.PersonTasks
{
    public class GettingCured : Task
    {

        Entity person;

        public GettingCured(Entity person)
        {
            this.person = person;
        }

        public override Result Run()
        {
            if(person.currentAction == Entity.CurrentAction.GettingCured)
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