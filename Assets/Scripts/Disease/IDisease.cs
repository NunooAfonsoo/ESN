using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts.Disease
{
    public interface IDisease
    {
        void Evolve();

        float GetHostTraitMultiplier(string name);

        bool ContainsTrait(string name);

        void ChangeTraitMultiplier(string name, float multiplier);

        void Mutate(List<string> traits, List<float> traitValues);
    }
}