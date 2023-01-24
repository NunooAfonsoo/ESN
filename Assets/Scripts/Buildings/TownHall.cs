using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Managers;
using Assets.Scripts.UI.TownHallUI;
using Assets.Scripts.Clock;

namespace Assets.Scripts.Buildings
{
    public class TownHall : Building
    {
        public static TownHall Instance;

        [SerializeField] private GameObject townHallMenuObject;

        public Dictionary<SensibilizationCampaign, float> CampaignsCompletionPercentage;
        [SerializeField] private ActiveCampaign[] activeCampaigns;
        public ActiveLaw activeLaw;

        protected override void Awake()
        {
            base.Awake();
            Instance = this;

            CampaignsCompletionPercentage = new Dictionary<SensibilizationCampaign, float>();

            activeCampaigns = new ActiveCampaign[4];
        }


        public void RegisterCampaign(int index, ActiveCampaign campaign)
        {
            activeCampaigns[index] = campaign;
        }

        public void RegisterLaw(ActiveLaw law)
        {
            activeLaw = law;
        }


        private void OnMouseDown()
        {
            if (UIManager.Instance.ActiveMenu == null && !GameManager.Instance.PauseMenuActive)
            {
                townHallMenuObject.SetActive(true);
                UIManager.Instance.ActiveMenu = townHallMenuObject;
            }
        }


        public IEnumerator ShowCampaignProgression(SensibilizationCampaign campaign, float startTime, float endTime)
        {
            endTime = endTime - startTime;
            while (CampaignsCompletionPercentage[campaign] < 1)
            {
                CampaignsCompletionPercentage[campaign] = 1 -((endTime - (ClockUI.Instance.day - startTime)) / endTime);

                yield return new WaitForSeconds(0.1f);
            }
            CampaignsCompletionPercentage[campaign] = 0;
            yield return null;
        }
    }
}
