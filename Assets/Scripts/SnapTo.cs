using UnityEngine;

namespace ASimpleRoguelike {
    public class SnapTo : MonoBehaviour {
        public Transform target;

        public void Update() {
            if (target == null) return;

            transform.position = target.position;
        }
    }
}