using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Clock;

namespace Assets.Scripts.DayNightCycle
{
    public class DayNightCycle : MonoBehaviour
    {
        private float timer;
        private float percentageOfDay;
        private float turnSpeed;

        [SerializeField] private Light sun;
        [SerializeField] private Light moon;

        [SerializeField] private Color32 daySkyTint;
        [SerializeField] private Color32 SunsetSkyTint;
        [SerializeField] private Color32 nightSkyTint;
        [SerializeField] private Color32 currentSkyTint;

        private float updateInterval;
        private float maxUpdateInterval;

        private void Start()
        {
            timer = 0.0f;
            currentSkyTint = daySkyTint;
            updateInterval = 0.0f;
            maxUpdateInterval = 0.15f;
        }

        private void Update()
        {
            updateInterval += Time.deltaTime;
            if(updateInterval > maxUpdateInterval)
            {
                updateInterval = 0.0f;
                CheckTime();
                UpdateLights();

            }
                if(IsNight())
                {
                    turnSpeed = 180.0f / 45 * Time.deltaTime;
                }
                else
                {
                    turnSpeed = 180.0f / 75 * Time.deltaTime;
                }

                transform.RotateAround(transform.position, transform.right, turnSpeed / 2);
        }



        private void UpdateLights()
        {
            Light light = GetComponent<Light>();
            if(IsSunSetTime())
            {
                /*  Setting the new colors for Evening sky  */
                if(currentSkyTint.r < SunsetSkyTint.r)
                {
                    currentSkyTint.r++;
                }
                if(currentSkyTint.g > SunsetSkyTint.g)
                {
                    currentSkyTint.g--;
                }
                if(currentSkyTint.b > SunsetSkyTint.b)
                {
                    currentSkyTint.b--;
                }

                RenderSettings.skybox.SetColor("_SkyTint", currentSkyTint);
            }
            else if(IsNight())
            {
                /*  Setting the new colors for night sky  */
                if(currentSkyTint.r > nightSkyTint.r)
                {
                    currentSkyTint.r -= (byte)3f;
                }
                if(currentSkyTint.g > nightSkyTint.g)
                {
                    currentSkyTint.g -= (byte)3f;
                }
                if(currentSkyTint.b > nightSkyTint.b)
                {
                    currentSkyTint.b -= (byte)3f;
                }

                RenderSettings.skybox.SetColor("_SkyTint", currentSkyTint);

                if(RenderSettings.sun != moon)
                {
                    sun.intensity = 0;
                    RenderSettings.sun = moon;//light.intensity -= 0.05f;
                }
            }
            else
            {
                /*  Setting the new colors for day sky  */
                if(currentSkyTint.r < daySkyTint.r)
                {
                    currentSkyTint.r++;
                }
                if(currentSkyTint.g < daySkyTint.g)
                {
                    currentSkyTint.g++;
                }
                if(currentSkyTint.b < daySkyTint.b)
                {
                    currentSkyTint.b++;
                }

                RenderSettings.skybox.SetColor("_SkyTint", currentSkyTint);

                if(light.intensity < 1.0f)
                {
                    light.intensity += 0.05f;
                }
                else
                {
                    RenderSettings.sun = sun;
                }
            }
        }

        private bool IsSunSetTime()
        {
            return percentageOfDay > 0.79f && percentageOfDay <= 0.875f;
        }

        private bool IsNight()
        {
            return percentageOfDay > 0.875f || percentageOfDay < 0.25f;
        }


        private void CheckTime()
        {
            timer += Time.deltaTime;
            percentageOfDay = (ClockUI.Instance.day % 2) / 2;//timer / (MinutesInDay * 60.0f);

            if(timer > ClockUI.REAL_SECONDS_PER_INGAME_DAY)
            {
                timer = 0.0f;
            }
        }
    }

}
