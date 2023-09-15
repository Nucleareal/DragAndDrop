using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace StellaDragAndDropNS
{
    public class ViewContextMenuItem : MonoBehaviour
    {
        private Color _baseColor;
        private EventTrigger _eventTrigger;
        private ContextMenuItem _logic;
        private GameCard _target;
        private Image _image;

        private void Awake()
        {
            _eventTrigger = gameObject.AddComponent<EventTrigger>();
            
            var pointerEnterEntry = new EventTrigger.Entry();
            var pointerExitEntry = new EventTrigger.Entry();
            var pointerClickEntry = new EventTrigger.Entry();
            
            pointerEnterEntry.eventID = EventTriggerType.PointerEnter;
            pointerExitEntry.eventID = EventTriggerType.PointerExit;
            pointerClickEntry.eventID = EventTriggerType.PointerClick;

            pointerEnterEntry.callback.AddListener(OnEnter);
            pointerExitEntry.callback.AddListener(OnExit);
            pointerClickEntry.callback.AddListener(OnClick);

            _eventTrigger.triggers.Add(pointerEnterEntry);
            _eventTrigger.triggers.Add(pointerExitEntry);
            _eventTrigger.triggers.Add(pointerClickEntry);
        }

        private void Start()
        {
            _image = GetComponent<Image>();
            _image.color = new Color(0f, 0f, 0f, 0f);
        }

        public void SetLogic(ContextMenuItem item)
        {
            _logic = item;
        }
        public void SetTarget(GameCard target)
        {
            _target = target;
        }

        private void OnEnter(BaseEventData e)
        {
            if (_logic.IsSeparator) return;

            _image.color = Color.white;
        }
        private void OnExit(BaseEventData e)
        {
            if (_logic.IsSeparator) return;

            _image.color = new Color(0f, 0f, 0f, 0f);
        }
        private void OnClick(BaseEventData e)
        {
            if (_logic.IsSeparator) return;
            if(e is PointerEventData p && p.button != PointerEventData.InputButton.Left) return;

            _logic.DoAction(_target);
            ContextMenuPatch.CloseMenu();
        }
    }
}
