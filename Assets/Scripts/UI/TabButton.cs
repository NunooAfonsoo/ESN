using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Assets.Scripts.UI
{
    public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
    {

        public TabGroup tabGroup;

        public RawImage ActiveTab;

        public Texture TabIdle;
        public Texture TabSelected;
           

        private void Start()
        {
            tabGroup.Subscribe(this);
            ActiveTab = GetComponent<RawImage>();
        }


        public virtual void OnPointerClick(PointerEventData eventData)
        {
            ActiveTab.texture = TabSelected;
            tabGroup.ActiveTabButton = this;
            ActiveTab.color = new Color(ActiveTab.color.r, ActiveTab.color.g, ActiveTab.color.b, 1f);
        }

        public void OnPointerEnter(PointerEventData eventData)
        { 
            if (ActiveTab.texture != TabSelected)
            {
                ActiveTab.texture = TabIdle;
                ActiveTab.color = new Color(ActiveTab.color.r, ActiveTab.color.g, ActiveTab.color.b, 0.8f);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (ActiveTab.texture != TabSelected)
            {
                ActiveTab.texture = TabIdle;
                ActiveTab.color = new Color(ActiveTab.color.r, ActiveTab.color.g, ActiveTab.color.b, 1f);
            }
        }

        public virtual void ResetTab()
        {
            ActiveTab.texture = TabIdle;
        }
    }
}