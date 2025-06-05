using UnityEngine;

namespace ASimpleRoguelike {
    public class RotateSelf : MonoBehaviour {
        public bool normalAxis = true;

        [Tooltip("Degrees per second")]
        public float rotateSpeed = 10f;

        [Tooltip("Can be paused")]
        public bool isPausable = true;
        
        void Update() {
            if (GlobalGameData.isPaused && isPausable) return;
            transform.Rotate(normalAxis ? Vector3.forward : Vector3.right, rotateSpeed * Time.deltaTime);
        }
    }
}