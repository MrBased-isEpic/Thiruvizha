using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Thiruvizha.UI
{
    public class Item : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
    {
        private bool canDrag = true;
        private RectTransform rect;
        private CanvasGroup canvasGroup;

        public Action OnDropped;
        public Action OnDragging;

        private void Awake()
        {
            rect = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            OnDragging?.Invoke();
            rect.anchoredPosition += eventData.delta;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            canvasGroup.blocksRaycasts = true;
            OnDropped?.Invoke();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            canvasGroup.blocksRaycasts = false;
        }

        private void OnDisable()
        {
            canvasGroup.blocksRaycasts = true;
        }
    }
}
