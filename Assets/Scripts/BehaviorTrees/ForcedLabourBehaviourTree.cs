using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.BehaviourTrees.PersonTasks;
using Assets.Scripts.Buildings;
using Assets.Scripts.Citizens;


namespace Assets.Scripts.BehaviourTrees
{
    class ForcedLabourBehaviourTree : Selector
    {
        public ForcedLabourBehaviourTree(Entity person, Home home, Transform homeDoor, Workplace workplace, Transform workplaceDoor, List<FreeTimePlace> freeTimepPlaces, Dictionary<FreeTimePlace, List<Transform>> freetimePlacesDoors, Hospital hospital, Transform hospitalDoor, Lab lab, Transform labDoor)
        {
            // To create a new tree you need to create each branch which is done using the constructors of different tasks
            // Additionally it is possible to create more complex behaviour by combining different tasks and composite tasks...
            int FTPIndex = Random.Range(0, freeTimepPlaces.Count);
            Transform FTPDoor = freetimePlacesDoors[freeTimepPlaces[FTPIndex]][Random.Range(0, freeTimepPlaces[FTPIndex].doorsTransform.Count)];
            person.CurrentBehaviourTree = Entity.BehaviourTreeType.ForcedLabour;
            this.children = new List<Task>()
            {
                new DoingFinalVaccine(person),
                new GoToFinalVaccine(person, lab, labDoor),
                new DoingVaccineTrial(person),
                new GoToVaccineTrial(person, lab, labDoor),
                new RestingAtHome(person),
                new GoToHome(person, home, homeDoor),
                new Working(person),
                new GoToWork(person, workplace, workplaceDoor),
                new LeisureTime(person),
                new GoToFTP(person, freeTimepPlaces[FTPIndex], FTPDoor)
            };

        }

    }
}
