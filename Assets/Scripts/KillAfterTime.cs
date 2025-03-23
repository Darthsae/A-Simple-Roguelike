using UnityEngine;

namespace ASimpleRoguelike {
    [RequireComponent(typeof(Health))]
    public class KillAfterTime : MonoBehaviour {
        public float killTime = 10f;
        private float timer = 0f;

        private void Update() {
            if (GlobalGameData.isPaused) return;
            
            timer += Time.deltaTime;
            if (timer > killTime) {
                GetComponent<Health>().ForceKill();
            }
        }
    }
}
