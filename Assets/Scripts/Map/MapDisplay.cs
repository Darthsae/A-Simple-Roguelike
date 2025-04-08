using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace ASimpleRoguelike.Map {
    public class MapDisplay : MonoBehaviour {
        public static List<MapScene> mapScenes = new();

        public PhaseManager phaseManager;
        public GameObject iconObject;
        public Node<IconElement> icons;
        public GameObject content;
        public List<int> flow = new() {0};
        public List<MapScene> displayedMaps = new();
        public List<MapScene> completed = new();

        public float Height() {
            return icons.Depth() * 120.0f;
        }

        [ContextMenu("Add Manual Flow")]
        void AddManualFlow() {
            AddFlow();
        }

        public void AddFlow() {
            List<MapScene> maps = new();

            WeakReference<Node<IconElement>> current = new(icons);

            for (int i = 1; i < flow.Count; i++) {
                current.SetTarget(current.TryGetTarget(out Node<IconElement> target) ? target.children[flow[i]] : null);
            }

            for (int i = 0; i < 3; i++) {
                MapScene thing = GetRandomValidMap(phaseManager.timerHandler.GetPhase(), maps);

                if (thing != null) {
                    maps.Add(thing);
                } else {
                    break;
                }
            }

            if (maps.Count == 0) {
                // IDK
                return;
            }

            float height = Height();
            float start = maps.Count == 1 ? 0 : (20 - maps.Count * 120) * 0.5f;
            float push = 120.0f;

            for (int i = 0; i < maps.Count(); i++) {
                GameObject thing = Instantiate(iconObject, content.transform);
                thing.GetComponent<RectTransform>().sizeDelta.Set(start + push * i, height);
                Node<IconElement> node = new() { data = thing.GetComponent<IconElement>() };
                node.data.Init(maps[i]);
                if (current.TryGetTarget(out Node<IconElement> target)) {
                    target.children.Append(node);
                }
            }

            displayedMaps = icons.AllValues().ConvertAll(X => X.map);
        }

        public MapScene GetRandomValidMap(int phase, List<MapScene> temp) {
            float totalWeight = 0.0f;

            List<MapScene> mapables = new();

            foreach (var map in mapScenes) {
                if (map.CanUse(this, phase) && !displayedMaps.Contains(map) && !temp.Contains(map)) {
                    mapables.Add(map);
                    totalWeight += map.weight;
                }
            }

            if (totalWeight == 0.0f) {
                return null;
            }

            float random = UnityEngine.Random.Range(0.0f, totalWeight);

            foreach (var map in mapables) {
                if (random < map.weight) {
                    return map;
                } else {
                    random -= map.weight;
                }
            }

            return null;
        }
    }

    [Serializable]
    public class Node<T> {
        public T data = default;
        public List<Node<T>> children = new();

        public int Depth() {
            int count = 1;
            int toAdd = 0;

            foreach (Node<T> child in children) {
                toAdd = math.max(toAdd, child.Depth());
            }

            return count + toAdd;
        }

        public List<T> AllValues() {
            List<T> values = new() {data};

            foreach (Node<T> child in children) {
                values.AddRange(child.AllValues());
            }

            return values;
        }
    }
}