using UnityEngine;
using UnityEngine.SceneManagement; // Required to work with scenes

namespace ASimpleRoguelike {
    public class Play : MonoBehaviour {
        // Call this method when the play button is clicked
        public void PlayGame(string sceneName) {
            // Replace "GameScene" with the name of the scene you want to load
            SceneManager.LoadScene(sceneName);
        }

        public void GameSelection(string sceneName) {
            string[] strings = sceneName.Split('-');
            if (GlobalGameData.unlockedEquinox || GlobalGameData.unlockedEquinox) {
                PlayGame(strings[0]);
            } else {
                PlayGame(strings[1]);
            }
        }
    }
}