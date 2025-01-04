using UnityEngine;

namespace ASimpleRoguelike {
    [CreateAssetMenu(fileName = "FactionData", menuName = "A Simple Roguelike/Faction", order = 1)]
    public class FactionData : ScriptableObject {
        [Tooltip("The sprite to use for this faction, 32x32 px")]
        public Sprite sprite;
        [Tooltip("The description of the faction")]
        public string description;
    }
}