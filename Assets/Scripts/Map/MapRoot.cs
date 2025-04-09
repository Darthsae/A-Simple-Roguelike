using ASimpleRoguelike.Entity;
using UnityEngine;

namespace ASimpleRoguelike.Map {
    public class MapRoot : MonoBehaviour {
        public GlobalGameData globalGameData;

        public void Init(GlobalGameData globalGameData) {
            this.globalGameData = globalGameData;
        }
        
        public void StartCutscene(int cutsceneIndex) {
            globalGameData.cameraController.StartCutscene(cutsceneIndex);
        }

        public void StartCutscene(string cutsceneName) {
            globalGameData.cameraController.StartCutscene(cutsceneName);
        }

        public void DisableSpawners() {
            globalGameData.phaseManager.DisableSpawners();
        }

        public void ClearMap() {
            globalGameData.phaseManager.ClearMap();
        }

        public void SetSpawning(bool spawning) {
            globalGameData.phaseManager.SetSpawning(spawning);
        }

        public void BossKilled(string newTrack) {
            globalGameData.cameraController.BossKilled(newTrack);
        }

        public void ForceKillPlayer() {
            globalGameData.player.health.ForceKill();
        }

        public void ClearEnemies() {
            Enemy.DespawnAll();
        }
    }
}