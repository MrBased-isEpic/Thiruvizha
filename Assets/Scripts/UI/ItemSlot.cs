using System;
using UnityEngine;


namespace Thiruvizha.UI
{
    public class ItemSlot : MonoBehaviour
    {
        public Action OnItemDraggedOutOfBounds;
        [SerializeField] private Item Item;

        private RectTransform itemTransform;
        private RectTransform rectTransform;

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            itemTransform = Item.GetComponent<RectTransform>();

            Item.OnDropped += ItemDropped;
            Item.OnDragging += ItemDragged;
        }

        private void OnEnable()
        {
            ItemDropped();
        }

        private void ItemDragged()
        {
            float xDiff = itemTransform.position.x - rectTransform.position.x;

            if (xDiff > 250)
            {
                //Debug.Log("Draggged out of Bounds");
                OnItemDraggedOutOfBounds?.Invoke();
            }
        }

        private void ItemDropped()
        {
            itemTransform.position = rectTransform.position;
        }
    }
}
