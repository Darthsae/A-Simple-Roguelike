using System.Collections.Generic;
using UnityEngine;

namespace ASimpleRoguelike {
    public class TaggedObject : MonoBehaviour {
        private HashSet<string> tags;

        public bool HasTag(string name) {
            return tags.Contains(name);
        }

        public void AddTag(string tag) {
            tags.Add(tag);
        }

        public void RemoveTag(string tag) {
            tags.Remove(tag);
        }
    }
}