using System;
using UnityEngine;

namespace Thiruvizha.UI
{
    public class PaletteUI : MonoBehaviour
    {
        [SerializeField] private ItemSlot[] itemSlots;
        public Action OnItemDragDetected;

        private void Start()
        {
            foreach (ItemSlot itemslot in itemSlots)
            {
                itemslot.OnItemDraggedOutOfBounds += ItemPicked;
            }
        }
        private void ItemPicked()
        {
            OnItemDragDetected();
        }
    }
}
