using ASimpleRoguelike.Dialogue;
using ASimpleRoguelike.Map;
using UnityEngine;
using UnityEngine.Events;

namespace ASimpleRoguelike {
    [RequireComponent(typeof(Collider2D))]
    public class EventOnEnter : MonoBehaviour {
        public MapRoot mapRoot;
        public UnityEvent events;
        public bool triggered = false;

        void Start() {
            mapRoot = FindFirstObjectByType<MapRoot>();
        }

        void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.TryGetComponent<Player>(out _)) {
                if (!triggered) {
                    events?.Invoke();
                }
                triggered = true;
            }
        }

        public void ToggleWater(bool water) {
            mapRoot.ToggleWater(water);
        }

        public void StartDialogue(DialogueScene a_dialogueScene) {
            mapRoot.StartDialogue(a_dialogueScene);
        }
        
        public void StartCutscene(int cutsceneIndex) {
            mapRoot.StartCutscene(cutsceneIndex);
        }

        public void StartCutscene(string cutsceneName) {
            mapRoot.StartCutscene(cutsceneName);
        }

        public void DisableSpawners() {
            mapRoot.DisableSpawners();
        }

        public void ClearMap() {
            mapRoot.ClearMap();
        }

        public void SetSpawning(bool spawning) {
            mapRoot.SetSpawning(spawning);
        }

        public void BossKilled(string newTrack) {
            mapRoot.BossKilled(newTrack);
        }

        public void ForceKillPlayer() {
            mapRoot.ForceKillPlayer();
        }
    }
}