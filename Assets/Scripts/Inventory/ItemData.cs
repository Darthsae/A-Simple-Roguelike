using UnityEngine;

namespace ASimpleRoguelike.Inventory {
    [CreateAssetMenu(menuName = "A Simple Roguelike/Item")]
    public class ItemData : ScriptableObject {
        public string[] tags;
        public string description;
        public Sprite sprite;
    }
}