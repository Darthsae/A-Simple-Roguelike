using UnityEngine;
using ASimpleRoguelike.Entity;

namespace ASimpleRoguelike {
    public class RotateAround : MonoBehaviour {
        [Tooltip("The object to rotate around")]
        public Transform rotateAroundTransform;

        [Tooltip("Degrees per second")]
        public float rotateSpeed = 10f;

        [Tooltip("The offset from the rotateAroundTransform, only really used for instant snapping")]
        public float offset = 0f;
        [Tooltip("The offset of rotation in degrees")]
        public float rotationOffset = 0f;
        [Tooltip("Instantly snap to the rotateAroundTransform")]
        public bool instantSnap = false;

        [Tooltip("Can be paused")]
        public bool isPausable = true;
        [Tooltip("Other rotation")]
        public float otherRotation = 0f;
        [Tooltip("Look at")]
        public bool lookAt = false;

        void Update() {
            if (GlobalGameData.isPaused && isPausable) return;

            if (instantSnap) {
                Vector3 pos = rotateAroundTransform.position + new Vector3(Mathf.Sin((Time.time * rotateSpeed + rotationOffset) * Mathf.Deg2Rad), Mathf.Cos((Time.time * rotateSpeed + rotationOffset) * Mathf.Deg2Rad), 0) * offset;
                transform.position = pos;
            } else if (rotateAroundTransform != null) {
                transform.RotateAround(rotateAroundTransform.position, Vector3.forward, rotateSpeed * Time.deltaTime);
            }

            if (lookAt) {
                transform.rotation = Quaternion.Euler(0, 0, Entity.Util.ActualAngle(transform.position, rotateAroundTransform.position) + otherRotation);
            }
        }
    }
}