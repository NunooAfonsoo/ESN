using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Buildings;

namespace Assets.Scripts.Managers
{
    public class BuildingsManager : MonoBehaviour
    {
        public static BuildingsManager Instance;

        public List<Transform> Homes;
        public List<Transform> Workplaces;
        public List<Transform> FreeTimePlaces;

        private void Awake()
        {
            Instance = this;
            Homes = new List<Transform>();
            Workplaces = new List<Transform>();
            FreeTimePlaces = new List<Transform>();
        }

        public void RegisterWorkPlace(Transform workplace)
        {
            Workplaces.Add(workplace);
        }

        public void RegisterHome(Transform home)
        {
            Homes.Add(home);
        }

        public void RegisterFTP(Transform FTP)
        {
            FreeTimePlaces.Add(FTP);
        }
    }
}
