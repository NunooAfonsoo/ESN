using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Citizens;

namespace Assets.Scripts.BehaviourTrees
{
    public class DoingVaccineTrial : Task
    {
        Entity person;

        public DoingVaccineTrial(Entity person)
        {
            this.person = person;
        }

        public override Result Run()
        {
            if (person.currentAction == Entity.CurrentAction.DoingVaccineTrial && person.TrialsOnHumans)
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
