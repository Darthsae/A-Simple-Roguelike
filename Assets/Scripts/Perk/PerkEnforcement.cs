using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ASimpleRoguelike.Map;

namespace ASimpleRoguelike.Perk {
    public class PerkEnforcement : MonoBehaviour {
        public MapRoot mapRoot;
        public List<PerkData> perks;
        public UnityEvent postRun;

        void Start() {
            mapRoot = FindFirstObjectByType<MapRoot>();
        }

        public void Run() {
            print("We are in");
            PerkManager perkManager = mapRoot.GetPerkManager();
            perkManager.tempEvent.AddListener(postRun.Invoke);
            print("Event set");
            perkManager.ForcedPerkChoice(perks);
            print("Forced");
        }
    }
}