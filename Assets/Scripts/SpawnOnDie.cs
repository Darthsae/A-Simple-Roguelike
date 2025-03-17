using UnityEngine;

namespace ASimpleRoguelike {
    [RequireComponent(typeof(Health))]
    public class SpawnOnDie : MonoBehaviour {
        public GameObject prefabToSpawn;
        public bool inheritRotation = false;

        void Start() {
            Health health = GetComponent<Health>();
            health.OnHealthZero += () => Instantiate(prefabToSpawn, transform.position, inheritRotation ? transform.rotation : Quaternion.identity);
        }
    }
}