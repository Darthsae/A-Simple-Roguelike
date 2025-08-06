using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor.UI;
using UnityEngine;

namespace ASimpleRoguelike.Map {
    public class MapDisplay : MonoBehaviour {
        public static List<MapScene> mapScenes = new();

        public PhaseManager phaseManager;
        public GameObject iconObject;
        public Node<IconElement> icons;
        public GameObject content;
        public List<int> flow = new() {};
        public List<MapScene> displayedMaps = new();
        public List<MapScene> completed = new();
        public GameObject map;

        public float Height() {
            print(icons.Depth());
            return icons.Depth() * 120.0f;
        }

        private void Start() {
            phaseManager.StartMap(icons.data.map);
            map.SetActive(false);
        }

        [ContextMenu("Add Manual Flow")]
        void AddManualFlow() {
            AddFlow();
        }

        public void AddFlow() {
            List<MapScene> maps = new();

            for (int i = 0; i < 3; i++) {
                MapScene thing = GetRandomValidMap(phaseManager.GetPhase(), maps);

                if (thing != null) {
                    maps.Add(thing);
                } else {
                    break;
                }
            }

            if (maps.Count == 0) {
                // IDK
                Debug.LogError("You still haven't implemented what happens when you have no maps.");
                return;
            }

            float height = Height();
            float start = (maps.Count - 1) * -60;
            float push = 120.0f;

            for (int i = 0; i < maps.Count(); i++) {
                GameObject thing = Instantiate(iconObject, content.transform);
                thing.GetComponent<RectTransform>().anchoredPosition = new(start + push * i, height);
                Node<IconElement> node = new() { data = thing.GetComponent<IconElement>() };
                node.data.Init(maps[i], this);
                icons.Append(flow, node);
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

        public void StartMap(MapScene map) {
            phaseManager.StartMap(map);
            foreach (IconElement element in icons.AllValues()) {
                if (!element.used) {
                    element.MakeUnusable();
                    displayedMaps.Remove(element.map);
                }
            }
        }
    }

    [Serializable]
    public class Node<T> {
        public T data = default;
        public List<Node<T>> children = new();

        public void Append(List<int> path, Node<T> add) {
            foreach (int thang in path) {
                Debug.Log("Ta: " + thang);
            }
            Debug.Log(add);
            if (path.Count == 0) {
                Debug.Log("Count 0");
                children.Add(add);
            } else if (children.Count > path[0]) {
                Debug.Log("Child Count");
                children[path[0]].Append(path.Where((num, ar) => { return ar != 0; }).ToList(), add);
            } else {
                Debug.Log("No child"); 
            }
        }

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