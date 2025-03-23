using UnityEngine;
using ASimpleRoguelike.Entity.Bosses;

namespace ASimpleRoguelike {
    public class PhaseManager : MonoBehaviour
    {
        public Player player;

        public Spawner[] spawnersForNormal;

        public CameraController cameraController;
        public TimerHandler timerHandler;

        public GameObject rotnot;
        public GameObject nyalanth;
        public GameObject lich;

        public void PhaseOne() {
            switch (GlobalGameData.gameContext) {
                case GameContext.Default:
                    cameraController.StartCutscene("Boss Cutscene", SpawnRotnot);
                    break;
            }
        }

        public void SpawnRotnot() { // Discord Equinox
            for (int i = 0; i < spawnersForNormal.Length; i++) {
                spawnersForNormal[i].spawning = false;
            }
            GameObject clone = Instantiate(rotnot, player.transform.position + (Vector3)Random.insideUnitCircle * 25f, Quaternion.identity);
            clone.transform.parent = null;
            RotnotController rotnotController = clone.GetComponent<RotnotController>();
            rotnotController.player = player.transform;

            float temp = timerHandler.timeScale;
            timerHandler.timeScale = 0f;
            
            rotnotController.health.OnHealthZero += () => {
                cameraController.StartCutscene("Cubenot Cutscene", () => {
                    GlobalGameData.unlockedEquinoxes[0] = true; 
                    GlobalGameData.unlockedEquinox = true;
                    GlobalGameData.unlockedItem = true;
                    GlobalGameData.unlockedItems[2] = true;

                    if (player.currentWeapon == WeaponType.Sword) {
                        GlobalGameData.unlockedItems[3] = true;
                    }
                }); 
                foreach (Spawner spawner in spawnersForNormal) {
                    spawner.spawning = true;
                } 
                cameraController.BossKilled("Post Rotnot");
                timerHandler.timeScale = temp;
            };
            
            cameraController.bossNotification = clone.GetComponent<BossNotification>();
            cameraController.SetBoss();
        }

        public void PhaseTwo() {
            switch (GlobalGameData.gameContext) {
                case GameContext.Default:
                    cameraController.StartCutscene("Boss Cutscene", SpawnNyalanth);
                    break;
            }
        }

        public void SpawnNyalanth() { // Redundant Equinox
            for (int i = 0; i < spawnersForNormal.Length; i++) {
                spawnersForNormal[i].spawning = false;
            }
            GameObject clone = Instantiate(nyalanth, player.transform.position + (Vector3)Random.insideUnitCircle * 25f, Quaternion.identity);
            clone.transform.parent = null;
            NyalanthController nyalanthController = clone.GetComponent<NyalanthController>();
            nyalanthController.player = player.transform;

            float temp = timerHandler.timeScale;
            timerHandler.timeScale = 0f;

            nyalanthController.health.OnHealthZero += () => {
                cameraController.StartCutscene("Nyalanth Cutscene", () => {
                    GlobalGameData.unlockedItems[1] = true;
                }); 
                foreach (Spawner spawner in spawnersForNormal) {
                    spawner.spawning = true;
                } 
                cameraController.BossKilled("Post Nyalanth");
                timerHandler.timeScale = temp;
            };
            
            cameraController.bossNotification = clone.GetComponent<BossNotification>();
            cameraController.SetBoss();
        }

        public void PhaseThree() {
            switch (GlobalGameData.gameContext) {
                case GameContext.Default:
                    cameraController.StartCutscene("Boss Cutscene", SpawnLich);
                    break;
            }
        }

        public void SpawnLich() { // Redundant Equinox
            for (int i = 0; i < spawnersForNormal.Length; i++) {
                spawnersForNormal[i].spawning = false;
            }
            GameObject clone = Instantiate(lich, player.transform.position + (Vector3)Random.insideUnitCircle * 25f, Quaternion.identity);
            clone.transform.parent = null;
            LichController lichController = clone.GetComponent<LichController>();
            lichController.player = player.transform;

            float temp = timerHandler.timeScale;
            timerHandler.timeScale = 0f;

            lichController.health.OnHealthZero += () => { // REMEMBER THIS IS FOR THE DEMO
                cameraController.StartCutscene("Nyalanth Cutscene", () => {
                    GlobalGameData.unlockedItems[0] = true;
                    player.health.ForceKill();
                }); 
                foreach (Spawner spawner in spawnersForNormal) {
                    spawner.spawning = true;
                } 
                cameraController.BossKilled("Post Lich");
                timerHandler.timeScale = temp;
            };
            
            cameraController.bossNotification = clone.GetComponent<BossNotification>();
            cameraController.SetBoss();
        }
    }

    public enum GameContext {
        Default
    }
}