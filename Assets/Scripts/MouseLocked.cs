using UnityEngine;

namespace ASimpleRoguelike {
    public class MouseLocked : MonoBehaviour
    {
        public bool isUiElement = false;
        public bool caresAboutTime = false;
        public Vector3 offset = Vector3.zero;

        public void Update() {
            if (caresAboutTime && GlobalGameData.isPaused)
                return;
            
            if (isUiElement) {
                // Get the mouse position in screen space
                Vector3 mousePosition = Input.mousePosition;

                // Set the position of the object to the mouse position
                transform.position = mousePosition + (Vector3)offset;
            } else {
                // Get the mouse position in world space
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                // Set the position of the object to the mouse position
                transform.position = mousePosition + (Vector3)offset;
            }
        }
    }
}