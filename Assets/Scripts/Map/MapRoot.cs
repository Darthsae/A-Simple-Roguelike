using ASimpleRoguelike.Dialogue;
using ASimpleRoguelike.Entity;
using ASimpleRoguelike.Perk;
using UnityEngine;

namespace ASimpleRoguelike.Map {
    public class MapRoot : MonoBehaviour {
        public GlobalGameData globalGameData;

        public void Init(GlobalGameData globalGameData) {
            this.globalGameData = globalGameData;
        }

        public void ToggleWater(bool water) {
            globalGameData.cameraController.SetWaterShader(water);
        }
        
        public void StartDialogue(DialogueScene a_dialogueScene) {
            globalGameData.dialogueManager.ChangeScene(a_dialogueScene);
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

        public PerkManager GetPerkManager() {
            return globalGameData.player.perkManager;
        }

        public void ClearEnemies() {
            Enemy.DespawnAll();
        }
    }
}