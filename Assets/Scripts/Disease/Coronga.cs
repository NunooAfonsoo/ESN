using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Citizens;

namespace Assets.Scripts.Disease
{
    public class Coronga : IDisease
    {
        protected string diseaseName;
        protected float condition;

        public static string[] TRAIT_KEYS = new string[] { "coughRate", "sweatRate", "diseaseResistance", "diseaseRadius", "diseaseAgressiveness", "diseaseRate" };

        public Dictionary<string, float> HostTraitMultipliers { get; private set; }

        public Coronga()
        {
            this.diseaseName = "Coronga";

            HostTraitMultipliers = new Dictionary<string, float>();

            HostTraitMultipliers["coughRate"] = 1f;
            HostTraitMultipliers["sweatingRate"] = 1f;
            HostTraitMultipliers["diseaseResistance"] = 1f;
            HostTraitMultipliers["diseaseRadius"] = 1.75f;
            HostTraitMultipliers["diseaseAgressiveness"] = 1f;
            HostTraitMultipliers["diseaseRate"] = 5f;            
        }

        public void Evolve()
        {
            
        }

        public float GetHostTraitMultiplier(string name)
        {
            return HostTraitMultipliers[name];                
        }

        public bool ContainsTrait(string name)
        {
            return HostTraitMultipliers.ContainsKey(name);
        }


        public void ChangeTraitMultiplier(string name, float multiplier)
        {
            HostTraitMultipliers[name] = multiplier;
        }



        public void Mutate(List<string> traits, List<float> traitValues)
        {
            for(int i = 0; i < traits.Count; i++)
            {
                HostTraitMultipliers[traits[i]] = traitValues[i];
            }
        }

    }
}
