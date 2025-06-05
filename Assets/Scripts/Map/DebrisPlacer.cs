using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace ASimpleRoguelike.Map {
    public class DebrisPlacer : MonoBehaviour {
        public GameObject debrisHolder;
        public List<DebrisData> data;
        public float distancer = 16f;

        public int check = 0;

        void Update() {
            check++;
            if (check > 25) {
                Collider2D[] smeg = { };
                if (Physics2D.OverlapCircleNonAlloc(transform.position, distancer, smeg) == 0) {
                    Vector3 pos = UnityEngine.Random.insideUnitCircle.normalized * distancer * 0.9f;
                    Instantiate(data[UnityEngine.Random.Range(0, data.Count)].gameObject, pos, Quaternion.identity, debrisHolder.transform);
                }
            }
        }
    }
}