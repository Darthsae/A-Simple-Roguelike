using UnityEngine;

namespace ASimpleRoguelike.Equinox {
    [CreateAssetMenu(fileName = "EquinoxData", menuName = "A Simple Roguelike/Equinox", order = 0)]
    public class EquinoxData : ScriptableObject {
        [Tooltip("The attunement of the equinox")]
        public EquinoxAttunement attunement;
        [Tooltip("The sprite to use for this equinox, 25x75 px")]
        public Sprite sprite;
        [Tooltip("The description of the equinox")]
        public string description;
    }
}