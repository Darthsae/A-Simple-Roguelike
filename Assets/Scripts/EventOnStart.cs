using ASimpleRoguelike.Entity.Bosses;
using ASimpleRoguelike.Map;
using UnityEngine;
using UnityEngine.Events;

namespace ASimpleRoguelike {
    public class EventOnStart : MonoBehaviour {
        public MapRoot mapRoot;
        public UnityEvent OnStart;

        private void Start() {
            mapRoot = FindFirstObjectByType<MapRoot>();
            OnStart?.Invoke();
        }
    }
}
