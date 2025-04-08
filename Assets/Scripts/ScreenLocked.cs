using UnityEngine;

namespace ASimpleRoguelike {
    [DefaultExecutionOrder(50)]
    public class ScreenLocked : MonoBehaviour {
        public bool isUiElement = false;
        public bool caresAboutTime = false;
        public Vector2 bottomLeftOffset = new(0,0);
        public Vector2 topRightOffset = new(1,1);
        public Vector2 bottomLeft => transform.position + bottomLeftOffset;
        public Vector2 topRight => transform.position + topRightOffset;

        public void Update() {
            if (caresAboutTime && GlobalGameData.isPaused)
                return;
            
            if (isUiElement) {
                Vector3 newPosition = transform.position;
                if (topRight.y > Screen.height) {
                    newPosition.y -= Screen.height - topRight.y;
                }
                if (topRight.x > Screen.width) {
                    newPosition.x -= Screen.width - topRight.x;
                }
                if (bottomLeft.y < 0) {
                    newPosition.y -= bottomLeft.y;
                }
                if (bottomLeft.x < 0) {
                    newPosition.x -= bottomLeft.x;
                }
                // Set the position of the object to the mouse position
                transform.position = newPosition;
            } else {
            }
        }
    }
}
