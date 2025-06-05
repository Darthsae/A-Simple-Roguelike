using System;
using System.Collections.Generic;
using UnityEngine;

namespace ASimpleRoguelike.Map {
    [CreateAssetMenu(menuName = "A Simple Roguelike/Map Scene")]
    public class MapScene : ScriptableObject {
        public IconData icon;
        public GameObject scene;
        public string description = "";
        public float weight = 1.0f;
        public int minPhase = 0;
        public int maxPhase = 2;
        public List<MapScene> prereqs = new();
        public List<DebrisData> debris = new();
        [Header("Map Scene Background")]
        public List<Background> backgrounds = new();

        [Header("Spawner Settings")]
        public List<SpawnHolder> spawnables = new();
        public Vector2 spawnRange = new(5f, 5f);
        public float spawnRate = 1f;
        public int maxSpawn = 10;

        public bool CanUse(MapDisplay mapDisplay, int phase) {
            if (phase < minPhase || phase > maxPhase) {
                return false;
            }
            foreach (MapScene map in prereqs) {
                if (mapDisplay.completed.FindIndex((map_) => map_ == map) == -1) {
                    return false;
                }
            }
            return true;
        }

        public void OnStart(GlobalGameData globalGameData) {
            foreach (Spawner spawner in globalGameData.phaseManager.spawnersForNormal) {
                spawner.spawnables = spawnables;
                spawner.spawnRange = spawnRange;
                spawner.spawnRate = spawnRate;
                spawner.maxSpawn = maxSpawn;
            }

            globalGameData.SetBackgrounds(backgrounds);
        }
    }

    [Serializable]
    public struct DebrisData {
        public GameObject gameObject;
        public float density;
    }

    [Serializable]
    public enum Background {
        Desert,
        Graveyard,
        Ocean
    }
}
