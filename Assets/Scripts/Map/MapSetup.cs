using System.Collections.Generic;
using UnityEngine;

namespace ASimpleRoguelike.Map {
    public class MapSetup : MonoBehaviour {
        public List<MapScene> maps;

        public void Setup() {
            MapDisplay.mapScenes = new List<MapScene>();

            foreach (var map in maps) {
                Debug.Log($"Map: {map.name}");
                MapDisplay.mapScenes.Add(map);
            }
        }
    }
}