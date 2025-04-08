using UnityEngine;

namespace ASimpleRoguelike {
    public class SpawnAfterTime : MonoBehaviour {
        public float spawnTime = 10f;
        private float timer = 0f;
        public GameObject prefabToSpawn;
        public Vector2 area = new(0f, 0f);
        public bool inheritRotation = false;

        private void Update() {
            if (GlobalGameData.isPaused) return;
            
            timer += Time.deltaTime;
            if (timer > spawnTime) {
                GameObject spawned = Instantiate(prefabToSpawn, transform.position + Spawner.InRange(area), inheritRotation ? transform.rotation : Quaternion.identity);
            }
        }
    }
}
