using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace ASimpleRoguelike {
    public class PointTowards : MonoBehaviour {
        // Look at mouse, this is easy
        public Transform target;

        public SpriteRenderer[] fills;
        public float distance = 8f;

        public float angleOffset = 0f;
        void Update() {
            // Mouse pos
            Vector3 mousePos = target.position;

            // Look at mouse
            float angleToMouse = Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x) * Mathf.Rad2Deg;
            transform.SetPositionAndRotation(transform.position, Quaternion.Euler(new Vector3(0, 0, angleToMouse + angleOffset)));
            for (int i = 0; i < fills.Length; i++) {
                if ((mousePos - transform.position).magnitude > distance) {
                    fills[i].color = Color.white;
                } else {
                    fills[i].color = Color.white.WithAlpha(0);
                }
            }
        }
    }
}