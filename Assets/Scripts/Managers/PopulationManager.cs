using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Citizens;

namespace Assets.Scripts.Managers
{
    public class PopulationManager : MonoBehaviour
    {
        public static PopulationManager Instance;

        [SerializeField] private GameObject[] personModels;
        public GameObject[] ScientistsModels;

        [SerializeField] int popSize;

        public List<Entity> Population;
        public List<Scientist> Scientists;
        public List<Entity> Infected;
        public int Deaths;

        public List<Entity> PopulationComplyingToLaw;

        public static event Action<float> OnHappinessChanged;
        public static event Action<float> OnProductivityChanged;
        public static event Action<float> OnDiseaseFearChanged;
        public static event Action<float> OnApprovalRatingChanged;

        public float AverageHappiness {
            get
            {
                float total = 0;
                foreach (Entity citizen in Population)
                {
                    total += citizen.Feelings.Happiness;
                }
                foreach (Scientist scientist in Scientists)
                {
                    total += scientist.Feelings.Happiness;
                }
                return (total / (Population.Count + Scientists.Count)) * 100;
            }
        }

        public float AverageProductivity
        {
            get
            {
                float total = 0;
                foreach (Entity citizen in Population)
                {
                    total += citizen.Feelings.Productivity;
                }
                foreach (Scientist scientist in Scientists)
                {
                    total += scientist.Feelings.Productivity;
                }
                return total / (Population.Count + Scientists.Count) * 100;
            }
        }


        public float AverageDiseaseFear
        {
            get
            {
                float total = 0;
                foreach (Entity citizen in Population)
                {
                    total += citizen.Feelings.DiseaseFear;
                }
                foreach (Scientist scientist in Scientists)
                {
                    total += scientist.Feelings.DiseaseFear;
                }
                return total / (Population.Count + Scientists.Count) * 100;
            }
        }


        public float AverageApprovalRating
        {
            get
            {
                float total = 0;
                foreach (Entity citizen in Population)
                {
                    total += citizen.Feelings.ApprovalRating;
                }
                foreach (Scientist scientist in Scientists)
                {
                    total += scientist.Feelings.ApprovalRating;
                }
                return total / (Population.Count + Scientists.Count) * 100;
            }
        }

        private void Awake()
        {
            Instance = this;
            Population = new List<Entity>();
            Scientists = new List<Scientist>();
            Infected = new List<Entity>();

            for (int i = 0; i < popSize; i++)
            {
                GameObject p = Instantiate(personModels[UnityEngine.Random.Range(0, personModels.Length)], Vector3.zero, Quaternion.identity);
                p.name += i.ToString();
            }
        }



        public void ChangedHappiness()
        {
            OnHappinessChanged?.Invoke(AverageHappiness);
        }

        public void ChangedProductivity()
        {
            OnProductivityChanged?.Invoke(AverageProductivity);
        }

        public void ChangedDiseaseFear()
        {
            OnDiseaseFearChanged?.Invoke(AverageDiseaseFear);
        }

        public void ChangedApprovalRating()
        {
            OnApprovalRatingChanged?.Invoke(AverageApprovalRating);
        }



        public void RegisterPerson(Entity person)
        {
            Population.Add(person);

            person.OnInfectedOrHealed += UIManager.Instance.UpdateInfectedNumbers;
            person.OnDeath += UIManager.Instance.UpdatePopulationNumbers;
            person.OnDeath += UIManager.Instance.UpdateDeathsNumbers;


        }

        public void RemovePerson(Entity person)
        { 
            Population.Remove(person);
            if (PopulationComplyingToLaw.Contains(person)) PopulationComplyingToLaw.Remove(person);
        }

        public void AddPersonComplyingToLaw(Entity person)
        {
            PopulationComplyingToLaw.Add(person);
        }

        public void RegisterScientist(Scientist scientist)
        {
            Scientists.Add(scientist);

            scientist.OnInfectedOrHealed += UIManager.Instance.UpdateInfectedNumbers;

            scientist.OnDeath += UIManager.Instance.UpdatePopulationNumbers;
            scientist.OnDeath += UIManager.Instance.UpdateScientistsNumbers;
            scientist.OnDeath += UIManager.Instance.UpdateDeathsNumbers;

            scientist.OnSpawn += UIManager.Instance.UpdateScientistsNumbers;
            scientist.OnSpawn += UIManager.Instance.UpdatePopulationNumbers;

        }

        public void RemoveScientist(Scientist scientist)
        {
            Scientists.Remove(scientist);
        }


        public void NewInfectedPerson(Entity person)
        {
            Infected.Add(person);
        }

        public void RemoveFromInfected(Entity person)
        {
            Infected.Remove(person);
        }
    }
}
