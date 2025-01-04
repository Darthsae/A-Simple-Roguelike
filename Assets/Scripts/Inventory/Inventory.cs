using UnityEngine;

namespace ASimpleRoguelike.Inventory {
    public class Inventory : MonoBehaviour
    {
        public int size;
        public GameObject slotTemplate;
        public Slot[] slots;

        public float offsetX = 200;

        public void Init() {
            slotTemplate.SetActive(true);
            float slotWidth = slotTemplate.GetComponent<RectTransform>().sizeDelta.x;
            float inventoryWidth = GetComponent<RectTransform>().sizeDelta.x;
            int xAmount = (int)(inventoryWidth / slotWidth);
            float padding = (inventoryWidth - slotWidth * xAmount) / (xAmount + 1);
            float slotHeight = slotTemplate.GetComponent<RectTransform>().sizeDelta.y;

            Debug.Log($"xAmount: {xAmount}, inventoryWidth: {inventoryWidth}, padding: {padding}, slotWidth: {slotWidth}, slotHeight: {slotHeight}");

            slots = new Slot[size];

            for (int i = 0; i < slots.Length; i++) {
                // Use a modulo with xAmount to get the x position of the slot, and use i / xAmount to get the y position of the slot
                float x = (i % xAmount) * (slotWidth + padding) + padding;
                float y = (i / xAmount) * (slotHeight + padding) + padding;
                Debug.Log($"Slot {i}: x: {x}, y: {y}");
                slots[i] = Instantiate(slotTemplate, transform).GetComponent<Slot>();
                slots[i].Init(this, x - offsetX, -y);
            }

            slotTemplate.SetActive(false);
        }

        public void QuickAdd(ItemData item, bool allowDuplicates) {
            if (!IsValid(item, allowDuplicates)) return;

            for (int i = 0; i < slots.Length; i++) {
                if (!slots[i].IsValid(item) || slots[i].item != null) continue;
                slots[i].SetItem(item);
                return;
            }
        }

        public void QuickRemove(ItemData item, bool deleteDuplicates) {
            for (int i = 0; i < slots.Length; i++) {
                if (slots[i].item == item) {
                    slots[i].SetItem(null);
                    if (!deleteDuplicates) return;
                }
            }
        }

        public bool IsValid(ItemData item, bool allowDuplicates) {
            for (int i = 0; i < slots.Length; i++) {
                if (slots[i].IsValid(item)) {
                    if (slots[i].item == null) {
                        return true;
                    } else if (!allowDuplicates && slots[i].item == item) {
                        return false;
                    }
                }
            }

            return false;
        }

        public bool HasItem(ItemData item) {
            for (int i = 0; i < slots.Length; i++) {
                if (slots[i].item == item) {
                    return true;
                }
            }

            return false;
        }
    }
}