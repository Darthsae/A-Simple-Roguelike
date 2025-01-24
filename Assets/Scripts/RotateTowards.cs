using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASimpleRoguelike {
    public class RotateTowards : MonoBehaviour {
        // Look at mouse, this is easy

        public float angleOffset = 0f;
        void Update() {
            // Mouse pos
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Look at mouse
            float angleToMouse = Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angleToMouse + angleOffset));
        }
    }
}