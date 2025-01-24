using UnityEngine;

namespace ASimpleRoguelike.Inventory {
    public class DefaultUnlocks : MonoBehaviour {
        public ItemHolder[] items = new ItemHolder[0];

        void Start () {
            for (int i = 0; i < items.Length; i++) {
                GlobalGameData.unlockedItems[items[i].id] = true;
            }
        }
    }

    [System.Serializable]
    public struct ItemHolder {
        public int id;
        public Item item;
    }
}