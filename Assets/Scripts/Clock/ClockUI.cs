using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Scripts.Managers;
using Assets.Scripts.Citizens;
using Assets.Scripts.UI.TownHallUI;
using System;

namespace Assets.Scripts.Clock
{
    public class ClockUI : MonoBehaviour
    {

        public static ClockUI Instance;

        public const float REAL_SECONDS_PER_INGAME_DAY = 120f;

        public Transform clockHourHandTransform;
        public Transform clockMinuteHandTransform;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI daysText;

        public float day;
        public float hoursValue;
        int textSunset;

        public int daysSurvived;

        private float lawStartTime;
        private bool testedLawCompliance;


        private void Awake()
        {
            Instance = this;
            day = 0.5f;
            textSunset = 200;
            daysSurvived = 0;

            lawStartTime = -1;
            testedLawCompliance = false;
        }


        private void Update()
        {
            day += Time.deltaTime / REAL_SECONDS_PER_INGAME_DAY;

            float dayNormalized = day % 2f;

            float rotationDegreesPerDay = 360f;
            clockHourHandTransform.eulerAngles = new Vector3(0, 0, -dayNormalized * rotationDegreesPerDay);

            float hoursPerDay = 12f;
            clockMinuteHandTransform.eulerAngles = new Vector3(0, 0, -dayNormalized * rotationDegreesPerDay * hoursPerDay);

            hoursValue = Mathf.Floor(dayNormalized * hoursPerDay);

            float minutesPerHour = 60f;
            float minutesValue = Mathf.Floor(((dayNormalized * hoursPerDay) % 1f) * minutesPerHour);

            timeText.text = hoursValue.ToString("00") + ":" + minutesValue.ToString("00");

            if(hoursValue >= 20 || hoursValue <= 6)
            {
                if(textSunset < 200) textSunset += 2;
                timeText.color = new Color32((byte)(50 + textSunset), (byte)(50 + textSunset), (byte)(50 + textSunset), 255);
                daysText.color = new Color32((byte)(50 + textSunset), (byte)(50 + textSunset), (byte)(50 + textSunset), 255);

                /*
                UIManager.Instance.AvrgHappiness.color = new Color32((byte)(50 + textSunset), (byte)(50 + textSunset), (byte)(50 + textSunset), 255);
                UIManager.Instance.AvrgProductiviy.color = new Color32((byte)(50 + textSunset), (byte)(50 + textSunset), (byte)(50 + textSunset), 255);
                UIManager.Instance.AvrgDiseaseFear.color = new Color32((byte)(50 + textSunset), (byte)(50 + textSunset), (byte)(50 + textSunset), 255);
                UIManager.Instance.AvrgApprovalRating.color = new Color32((byte)(50 + textSunset), (byte)(50 + textSunset), (byte)(50 + textSunset), 255);
                */

            }
            else
            {
                if(textSunset > 1) textSunset -= 2;
                timeText.color = new Color32((byte)(50 + textSunset), (byte)(50 + textSunset), (byte)(50 + textSunset), 255);
                daysText.color = new Color32((byte)(50 + textSunset), (byte)(50 + textSunset), (byte)(50 + textSunset), 255);

                /*
                UIManager.Instance.AvrgHappiness.color = new Color32((byte)(50 + textSunset), (byte)(50 + textSunset), (byte)(50 + textSunset), 255);
                UIManager.Instance.AvrgProductiviy.color = new Color32((byte)(50 + textSunset), (byte)(50 + textSunset), (byte)(50 + textSunset), 255);
                UIManager.Instance.AvrgDiseaseFear.color = new Color32((byte)(50 + textSunset), (byte)(50 + textSunset), (byte)(50 + textSunset), 255);
                UIManager.Instance.AvrgApprovalRating.color = new Color32((byte)(50 + textSunset), (byte)(50 + textSunset), (byte)(50 + textSunset), 255);
                */
            }

            daysText.text = "Days Survived: " + daysSurvived.ToString();


            if(lawStartTime != -1 && Mathf.Abs(dayNormalized - lawStartTime) < 0.01f && !testedLawCompliance)
            {
                if(PopulationManager.Instance.PopulationComplyingToLaw.Count < PopulationManager.Instance.Population.Count)
                {
                    testedLawCompliance = true;
                    Invoke("ResetTestedLawCompliance", 10f);
                    foreach (Entity person in PopulationManager.Instance.Population)
                    {
                        if(!PopulationManager.Instance.PopulationComplyingToLaw.Contains(person))
                        {
                            float prob = UnityEngine.Random.value;

                            if (prob <= person.Feelings.ApprovalRating)
                            {
                                person.ChangeBehaviorTree(GetActiveLawToRoutine());
                                PopulationManager.Instance.AddPersonComplyingToLaw(person);
                            }
                        }
                    }
                }
            }
        }

        private Entity.BehaviourTreeType GetActiveLawToRoutine()
        {
            if (PopulationManager.Instance.PopulationComplyingToLaw.Count > 0)
                return PopulationManager.Instance.PopulationComplyingToLaw[0].CurrentBehaviourTree;

            else
            {
                switch (TownHallUIManager.Instance.ActiveLaw)
                {
                    case Law.LawOptions.Lockdown:
                        return Entity.BehaviourTreeType.Lockdown;
                    case Law.LawOptions.RemoteWork:
                        return Entity.BehaviourTreeType.RemoteWork;
                    case Law.LawOptions.ForcedLabour:
                        return Entity.BehaviourTreeType.ForcedLabour;
                }
                return Entity.BehaviourTreeType.NormalRoutine;
            }
        }

        private void ResetTestedLawCompliance()
        {
            testedLawCompliance = false;
        }


        public IEnumerator SetLawStartTime(float dayTime)
        {
            float startTime = dayTime % 2f;
            yield return new WaitForSecondsRealtime(7f);
            lawStartTime = startTime;
            yield return null;
        }
    }
}