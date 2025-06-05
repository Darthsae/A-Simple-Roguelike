using System;
using UnityEngine;
using ASimpleRoguelike.Map;
using ASimpleRoguelike.Entity;
using System.Collections.Generic;
using ASimpleRoguelike.Inventory;
using ASimpleRoguelike.Equinox;

namespace ASimpleRoguelike {
    public class PhaseManager : MonoBehaviour {
        private int currentPhase = 0;
        public int GetPhase() => currentPhase;

        public Player player;
        public GlobalGameData globalGameData;
        public CameraController cameraController;
        public TimerHandler timerHandler;

        public MapScene currentMapScene;
        public GameObject sceneObject;

        public GameObject map;
        public MapDisplay mapDisplay;

        public Spawner[] spawnersForNormal;

        void Start() {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().OnDie += () => {
                GlobalGameData.longestTime = Math.Max(GlobalGameData.longestTime, timerHandler.time); 
                GlobalGameData.highestPhase = Math.Max(GlobalGameData.highestPhase, currentPhase);
            };
        }

        public void AdvanceScene(int phases, List<DataPrereq<ItemData>> items, List<DataPrereq<EquinoxData>> equinoxes) {
            currentPhase += phases;
            GlobalGameData.UnlockItems(globalGameData, items);
            GlobalGameData.UnlockEquinoxes(globalGameData, equinoxes);
            if (currentPhase > 0) {
                GlobalGameData.AddPauseReason("Map");
                Cursor.visible = true;
                map.SetActive(true);
                mapDisplay.AddFlow();
            }
        }

        public void StartMap(MapScene mapScene) {
            Vector3 position = player.transform.position;
            Quaternion rotation = Quaternion.identity;
            currentMapScene = mapScene;
            ClearMap();
            sceneObject = Instantiate(currentMapScene.scene, position, rotation);
            sceneObject.GetComponent<MapRoot>().Init(globalGameData);
            currentMapScene.OnStart(globalGameData);
            map.SetActive(false);
            SetSpawning(true);
        }

        public void DisableSpawners() {
            foreach (Spawner spawner in spawnersForNormal) {
                spawner.SetSpawning(false);
                spawner.Despawn();
            }
        }

        public void SetSpawning(bool spawning) {
            foreach (Spawner spawner in spawnersForNormal) {
                spawner.SetSpawning(spawning);
            }
        }

        public void ClearMap() {
            Enemy.DespawnAll();
            Destroy(sceneObject);
        }

        /*
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
        */
    }

    public enum GameContext {
        Default
    }
}