using UnityEngine;

namespace ASimpleRoguelike {
    public class Quit : MonoBehaviour {
        public void QuitGame() {
            if (Logger.open) {
                Logger.StopLogging();
            }

            Application.Quit();
        }
    }
}