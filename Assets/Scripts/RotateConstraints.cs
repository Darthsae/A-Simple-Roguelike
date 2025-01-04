using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASimpleRoguelike {
    public class RotateConstraints : MonoBehaviour
    {
        [Tooltip("Degrees per second")]
        public float rotateSpeed = 10f;

        [Tooltip("Start")]
        public float start = 0f;

        [Tooltip("End")]
        public float end = 0f;

        private float pos;

        int direction = 1;

        [Tooltip("Can be paused")]
        public bool isPausable = true;
        
        void Update()
        {
            if (GlobalGameData.isPaused && isPausable) return;

            pos += Time.deltaTime * direction;

            if (pos > end) {
                pos = rotateSpeed;
                direction = -1;
            } else if (pos < start) {
                pos = 0f;
                direction = 1;
            }

            transform.localRotation = Quaternion.Euler(0, 0, Mathf.LerpAngle(start, end, pos / rotateSpeed));
        }
    }
}