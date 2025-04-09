using ASimpleRoguelike.Map;
using UnityEngine;
using UnityEngine.Events;

namespace ASimpleRoguelike {
    [RequireComponent(typeof(Health))]
    public class EventOnDie : MonoBehaviour {
        public MapRoot mapRoot;
        public UnityEvent events;

        void Start() {
            Health health = GetComponent<Health>();
            health.OnHealthZero += () => events?.Invoke();
            mapRoot = FindFirstObjectByType<MapRoot>();
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