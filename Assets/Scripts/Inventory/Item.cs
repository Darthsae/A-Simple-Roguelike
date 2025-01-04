using System.Collections.Generic;
using UnityEngine;

namespace ASimpleRoguelike.Inventory {
    public class Item : MonoBehaviour {
        public static List<ItemData> items;
        public static int ItemCount => items.Count;
        public ItemData item;
    }
}