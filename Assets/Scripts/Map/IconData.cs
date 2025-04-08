using UnityEngine;

namespace ASimpleRoguelike.Map {
    [CreateAssetMenu(menuName = "A Simple Roguelike/Icon")]
    public class IconData : ScriptableObject {
        public Sprite frame;
        public Sprite icon;
    }
}