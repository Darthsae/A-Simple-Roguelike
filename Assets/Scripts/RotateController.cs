using UnityEngine;

namespace ASimpleRoguelike {
    public class RotateController : MonoBehaviour {
        [Tooltip("Degrees per second")]
        public float rotateSpeed = 10f;

        [Tooltip("Start angle (in degrees)")]
        public float start = 0f;

        [Tooltip("End angle (in degrees)")]
        public float end = 90f;

        private float currentAngle; // Current angle of rotation
        private bool running = false; // Is rotation active
        private int direction = 1; // 1 for forward, -1 for reverse

        [Tooltip("Can rotation be paused")]
        public bool isPausable = true;

        public void Rotate(float speed) {
            if (speed <= 0f) {
                Debug.LogWarning("Speed must be positive.");
                return;
            }

            rotateSpeed = (end - start) * 2f / speed;
            running = true;
            currentAngle = start; // Start at the initial angle
            direction = (int)Mathf.Sign(end - start); // Determine the shortest rotation direction
        }

        void Update() {
            if ((GlobalGameData.isPaused && isPausable) || !running) return;

            // Calculate the angle to move this frame
            float angleStep = rotateSpeed * Time.deltaTime * direction;

            // Update the current angle
            currentAngle += angleStep;

            // Handle overshooting the bounds
            if (direction > 0 && currentAngle >= end) {
                currentAngle = end;
                direction = -1; // Reverse direction
            } else if (direction < 0 && currentAngle <= start) {
                currentAngle = start;
                direction = 1; // Reverse direction
                running = false; // Stop rotation after one cycle
            }

            // Apply the rotation
            transform.localRotation = Quaternion.Euler(0, 0, currentAngle);
        }
    }
}
