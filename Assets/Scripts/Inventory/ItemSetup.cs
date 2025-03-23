using System.Collections.Generic;
using UnityEngine;

namespace ASimpleRoguelike.Inventory {
    public class ItemSetup : MonoBehaviour {
        public List<ItemData> items;

        public void Setup() {
            Item.items = new List<ItemData>();

            foreach (var item in items) {
                Debug.Log($"Item: {item.name}");
                Item.items.Add(item);
            }

            Debug.Log($"Item count: {Item.items.Count}");
            Debug.Log($"Item count: {Item.ItemCount}");
        }
    }
}