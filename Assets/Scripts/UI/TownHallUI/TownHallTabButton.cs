using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Assets.Scripts.Managers;


namespace Assets.Scripts.UI.TownHallUI
{
    public class TownHallTabButton : TabButton
    {

        public enum TownHallTab
        {
            Routine,
            Sensibilization,
            Law,
            Budget
        }

        public TownHallTab Tab;
        public GameObject TabObject;

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            TownHallUIManager.Instance.TownHallIntroText.SetActive(false);


            tabGroup.ResetOtherTabs(this);
            TabObject.SetActive(true);

        }


        public override void ResetTab()
        {
            base.ResetTab();

            TownHallUIManager.Instance.CloseDescriptions();
            TabObject?.SetActive(false);
        }
    }
}