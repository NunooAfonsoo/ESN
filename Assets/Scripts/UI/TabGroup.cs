using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class TabGroup : MonoBehaviour
    {
        public List<TabButton> TabButtons;

        public TabButton ActiveTabButton;
        
        public void Subscribe(TabButton tabButton)
        {
            if(TabButtons == null) TabButtons = new List<TabButton>();

            TabButtons.Add(tabButton);
        }


        public void ResetOtherTabs(TabButton tabButton)
        {
            foreach(TabButton button in TabButtons)
            {
                if(button != tabButton)
                {
                    button.ResetTab();
                }

            }
        }


        public void ResetTabs()
        {
            foreach (TabButton button in TabButtons)
            {
                button.ResetTab();
            }
        }
    }
}