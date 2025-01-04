using UnityEngine;

namespace ASimpleRoguelike {
    [RequireComponent(typeof(Health))]
    public class SpawnOnDie : MonoBehaviour {
        public GameObject prefabToSpawn;

        void Start() {
            Health health = GetComponent<Health>();
            health.OnHealthZero += () => Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
        }
    }
}