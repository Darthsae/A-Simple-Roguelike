using UnityEngine;

namespace ASimpleRoguelike.StatusEffects {
    [CreateAssetMenu(menuName = "A Simple Roguelike/StatusEffect")]
    public class StatusEffectData : ScriptableObject {
        public string[] tags;
        public string description;
        public Sprite sprite;
        public float time = 2.5f;
        public float timerDamageTime = 0.5f;
        public float damage = 0;
        public float speed = 0;
        public GameObject toSpawn;
    }
}