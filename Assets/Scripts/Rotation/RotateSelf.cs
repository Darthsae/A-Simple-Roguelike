using UnityEngine;

namespace ASimpleRoguelike {
    public class RotateSelf : MonoBehaviour {
        [Tooltip("Degrees per second")]
        public float rotateSpeed = 10f;

        [Tooltip("Can be paused")]
        public bool isPausable = true;
        
        void Update() {
            if (GlobalGameData.isPaused && isPausable) return;
            transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
        }
    }
}