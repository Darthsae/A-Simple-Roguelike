using UnityEngine;

namespace ASimpleRoguelike {
    [DefaultExecutionOrder(50)]
    public class ScreenLocked : MonoBehaviour {
        public bool isUiElement = false;
        public bool caresAboutTime = false;
        public Vector2 bottomLeftOffset = new(0,0);
        public Vector2 topRightOffset = new(1,1);
        public Vector2 BottomLeft => (Vector2)transform.position + bottomLeftOffset;
        public Vector2 TopRight => (Vector2)transform.position + topRightOffset;

        public void Update() {
            if (caresAboutTime && GlobalGameData.isPaused)
                return;
            
            if (isUiElement) {
                Vector3 newPosition = transform.position;
                if (TopRight.y > Screen.height) {
                    newPosition.y -= Screen.height - TopRight.y;
                }
                if (TopRight.x > Screen.width) {
                    newPosition.x -= Screen.width - TopRight.x;
                }
                if (BottomLeft.y < 0) {
                    newPosition.y -= BottomLeft.y;
                }
                if (BottomLeft.x < 0) {
                    newPosition.x -= BottomLeft.x;
                }
                transform.position = newPosition;
            } else {
            }
        }
    }
}
