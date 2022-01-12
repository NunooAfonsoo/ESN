using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace Assets.Scripts.UI.HospitalUI
{
    [RequireComponent(typeof(RawImage))]
    public class HospitalTabButton : TabButton
    {
        public GameObject Unit;

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            HospitalUIManager.Instance.HospitalIntroText.SetActive(false);

            if (name == "NormalUnit")
            {
                if (HospitalUIManager.Instance.PersonInfoNUDict.Keys.Count == 0)
                    HospitalUIManager.Instance.SetEmptyUnitText(name, true);
                else
                {
                    HospitalUIManager.Instance.SetEmptyUnitText(name, false);
                    foreach (string key in HospitalUIManager.Instance.PersonInfoNUDict.Keys)
                    {
                        HospitalUIManager.Instance.PersonInfoNUDict[key].SetActive(true);
                    }
                }
            }
            else if (name == "ICU")
            {
                if (HospitalUIManager.Instance.PersonInfoICUDict.Keys.Count == 0)
                    HospitalUIManager.Instance.SetEmptyUnitText(name, true);
                else
                {
                    HospitalUIManager.Instance.SetEmptyUnitText(name, false);
                    foreach (string key in HospitalUIManager.Instance.PersonInfoICUDict.Keys)
                    {
                        HospitalUIManager.Instance.PersonInfoICUDict[key].SetActive(true);
                    }
                }
            }

            Unit.SetActive(true);
            Unit.transform.GetChild(0).GetComponent<ScrollRect>().verticalNormalizedPosition = 1;
            tabGroup.ResetOtherTabs(this);
        }

        public override void ResetTab()
        {
            base.ResetTab();
            Unit.SetActive(false);
        }

    }

}