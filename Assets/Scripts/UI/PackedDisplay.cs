using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ASimpleRoguelike.UI {
    public class PackedDisplay : UIBehaviour {
        [ContextMenu("Recalculate")]
        void RecalculateOption() {
            Recalculate();
        }

        public List<GameObject> toDisplay = new();
        public bool horizontal = true;
        public bool vertical = true;
        public RectTransform content;

        public void AddItem(GameObject item) {
            toDisplay.Add(item);
            Recalculate();
        }

        public void AddItemNoCalculate(GameObject item) {
            toDisplay.Add(item);
        }

        public void RemoveItem(GameObject item) {
            toDisplay.Remove(item);
            Recalculate();
        }

        public void Clear() {
            foreach (GameObject item in toDisplay) {
                Destroy(item);
            }
            toDisplay.Clear();
        }

        public void Recalculate() {
            Debug.Log("Recalculating");
            if (horizontal) {
                Debug.Log("Horizontal");
                float xOffset = 0;
                for (int i = 0; i < toDisplay.Count; i++) {
                    RectTransform rect = toDisplay[i].GetComponent<RectTransform>();
                    rect.anchoredPosition = new(xOffset, 0);
                    Debug.Log("Position: " + rect.anchoredPosition);
                    Debug.Log("Size: " + rect.sizeDelta.x);
                    xOffset += rect.sizeDelta.x;
                }
                Debug.Log("This is the size: " + xOffset);
                content.sizeDelta.Set(xOffset, content.sizeDelta.y);
            } else if (vertical) {
                Debug.Log("Vertical");
                float yOffset = 0;
                for (int i = 0; i < toDisplay.Count; i++) {
                    RectTransform rect = toDisplay[i].GetComponent<RectTransform>();
                    rect.anchoredPosition = new(0, -yOffset);
                    Debug.Log("Position: " + rect.anchoredPosition);
                    Debug.Log("Size: " + rect.sizeDelta.y);
                    yOffset += rect.sizeDelta.y;
                }
                Debug.Log("This is the size: " + yOffset);
                content.sizeDelta = new(content.sizeDelta.x, yOffset);
            }
        }
    }
}