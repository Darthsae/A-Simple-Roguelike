using UnityEngine;

namespace ASimpleRoguelike {
    public class ScreenStuff : MonoBehaviour {
        // Call this method when the play button is clicked
        public void ToggleFullscreen() {
            Screen.fullScreen = !Screen.fullScreen;
        }
    }
}