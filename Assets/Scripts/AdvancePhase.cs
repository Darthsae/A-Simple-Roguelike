
using System.Collections.Generic;
using ASimpleRoguelike.Equinox;
using ASimpleRoguelike.Inventory;
using ASimpleRoguelike.Map;
using UnityEngine;

namespace ASimpleRoguelike {
    public class AdvancePhase : MonoBehaviour {
        public MapRoot mapRoot;
        public int phases;
        public List<DataPrereq<ItemData>> items = new();
        public List<DataPrereq<EquinoxData>> equinoxes = new();

        private void Start() {
            mapRoot = FindFirstObjectByType<MapRoot>();
        }

        public void AdvanceScene() {
            mapRoot.globalGameData.phaseManager.AdvanceScene(phases, items, equinoxes);
        }
    }
}