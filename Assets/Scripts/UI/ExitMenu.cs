using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Assets.Scripts.Managers;

namespace Assets.Scripts.UI
{
    public class ExitMenu : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            UIManager.Instance.ActiveMenu.SetActive(false);
            UIManager.Instance.ActiveMenu = null;
        }
    }
}