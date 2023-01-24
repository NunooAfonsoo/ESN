using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Managers;
using Assets.Scripts.UI.TownHallUI;


namespace Assets.Scripts.Citizens
{
    [System.Serializable]
    public class Feelings
    {
        public float Happiness;
        public float Productivity;
        public float DiseaseFear;
        public float ApprovalRating;

        public Feelings(float Happiness, float Productivity, float DiseaseFear, float ApprovalRating)
        {
            this.Happiness = Happiness;
            this.Productivity = Productivity;
            this.DiseaseFear = DiseaseFear;
            this.ApprovalRating = ApprovalRating;
        }

        public void UpdateHappiness(float Workshift = -1, float happinessUpdate = 0)
        {
            float oldHappiness = Happiness;
            if (Workshift != -1)
            {
                int approvalRatingTimes = TownHallUIManager.Instance.GetSensibilizationTimes(SensibilizationCampaign.SensibilizationCampaignOptions.ApprovalRatingBoost);
                Happiness = UnityEngine.Random.Range(1 - Workshift - 0.15f, 1 - Workshift + 0.15f) + (float)approvalRatingTimes * UnityEngine.Random.Range(0.05f, 0.15f);
            }
            else
            {
                Happiness += happinessUpdate;
            }

            if (Happiness > 1.0f) Happiness = 1.0f;
            if (Happiness < 0.0f) Happiness = 0.0f;

            PopulationManager.Instance.ChangedHappiness();

            float approvalRatingUpdate = UnityEngine.Random.Range(0.5f, 2.0f);

            UpdateApprovalRating(approvalRatingUpdate  * -(oldHappiness - Happiness));

            UpdateProductivity();
        }


        public void UpdateProductivity()
        {
            Productivity = 0.6f * Happiness + 0.4f * ApprovalRating;

            if (Productivity > 1.0f) Productivity = 1.0f;
            if (Productivity < 0.0f) Productivity = 0.0f;

            PopulationManager.Instance.ChangedProductivity();
        }


        public void UpdateDiseaseFear(float diseaseFearUpdate)
        {
            DiseaseFear += diseaseFearUpdate;

            if (DiseaseFear > 1.0f) DiseaseFear = 1.0f;
            if (DiseaseFear < 0.0f) DiseaseFear = 0.0f;

            PopulationManager.Instance.ChangedDiseaseFear();
        }

        public void UpdateApprovalRating(float approvalRatingUpdate)
        {
            ApprovalRating += approvalRatingUpdate;

            if (ApprovalRating > 1.0f) ApprovalRating = 1.0f;
            if (ApprovalRating < 0.0f) ApprovalRating = 0.0f;

            PopulationManager.Instance.ChangedApprovalRating();
        }
    }
}
