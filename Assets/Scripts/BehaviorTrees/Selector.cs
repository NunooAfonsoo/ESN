using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts.BehaviourTrees
{

    public class Selector : CompositeTask
    {
        public Selector(List<Task> tasks) : base(tasks)
        {
        }

        public Selector() { }

        // A Selector will choose the first action to succeed
        // If all actions fail the Selector fails
        public override Result Run()
        {
            if(children.Count > this.currentChild)
            {
                Result result = children[currentChild].Run();

                if(result == Result.Running)
                    return Result.Running;

                else if(result == Result.Success)
                {
                    currentChild = 0;
                    return Result.Success;
                }

                else
                {
                    currentChild++;
                    if(children.Count > this.currentChild)
                        return Result.Running;
                    else
                    {
                        // all tasks have been checked and none returned success
                        currentChild = 0;
                        return Result.Failure;
                    }
                }
            }

            return Result.Success;
        }

    }
}
