using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ASimpleRoguelike.Inventory {
    public class Slot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
        public ItemData item;
        public Image image;
        public List<string> validTags;
        public bool blacklist = true;

        public event Action<ItemData> OnItemChanged;

        public Inventory inventory;

        public void SetItem(ItemData newItem) {
            item = newItem;
            if (item != null && item.sprite != null) {
                image.sprite = item.sprite;
                image.gameObject.SetActive(true);
            } else {
                image.gameObject.SetActive(false);
            }
        }

        public void OnPointerClick(PointerEventData eventData) {
            //Debug.Log($"Mouse down slot {item?.name}");

            if (GlobalGameData.item == null && item != null) {
                GlobalGameData.SetItem(item);
                SetItem(null);
            } else if (IsValid(GlobalGameData.item)) {
                ItemData tempItem = item;
                SetItem(GlobalGameData.item);
                GlobalGameData.SetItem(tempItem);
            }

            OnItemChanged?.Invoke(item);
        }

        public void OnPointerExit(PointerEventData eventData) { 
            if (eventData.fullyExited) 
                GlobalGameData.tooltip.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData) {
            //Debug.Log($"Mouse over slot {item?.name}");
            if (GlobalGameData.item == null && item != null) {
                // Show tooltip here. Do some fancy stuff to make the name bold and description in italic on a newline
                GlobalGameData.tooltipText.text = "<b>" + item.name + "</b>\n<i>" + item.description + "</i>";
                GlobalGameData.tooltip.SetActive(true);
            } else {
                GlobalGameData.tooltip.SetActive(false);
            }
        }

        public bool IsValid(ItemData item) {
            if (item == null) return false;

            foreach (string tag in item.tags) if (validTags.Contains(tag)) return !blacklist;

            return blacklist;
        }

        public void Init(Inventory inventory, float x, float y) {
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.localPosition = new Vector2(x, y);
            SetItem(null);
            this.inventory = inventory;
        }
    }
}