using UnityEngine;

namespace ASimpleRoguelike {
    public class SpawnAfterTime : MonoBehaviour {
        public float spawnTime = 10f;
        private float timer = 0f;

        private void Update() {
            if (GlobalGameData.isPaused) return;
            
            timer += Time.deltaTime;
            if (timer > spawnTime) {
                
            }
        }
    }
}
