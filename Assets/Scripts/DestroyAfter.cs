using UnityEngine;

namespace ASimpleRoguelike {
    public class DestroyAfter : MonoBehaviour {
        public float time = 1f;
        public float delay = 0f;

        // Update is called once per frame
        void Update() {
            if (GlobalGameData.isPaused) return;

            delay += Time.deltaTime;
            if (delay >= time)
                Destroy(gameObject);
        }
    }
}