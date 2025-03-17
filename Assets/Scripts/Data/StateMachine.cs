using System;
using System.Collections.Generic;
using UnityEngine;

namespace ASimpleRoguelike.Data {
    [Serializable]
    public class StateMachine : ISerializationCallbackReceiver {
        public Dictionary<string, State> StateMap = new();


        public void RenameState(string name, string newName) {
            StateMap[newName] = StateMap[name].Update(newName);
            StateMap.Remove(name);
            foreach (var pair in StateMap) {
                pair.Value.RenameState(name, newName);
            }
        }

        #region Serialization
        public void OnAfterDeserialize() {
        }

        public void OnBeforeSerialize() {
        }
        #endregion
    }

    [Serializable]
    public class State {
        public string Name;
        public HashSet<string> Transitions;

        public State Update(string name) {
            Name = name;
            return this;
        }

        public void RenameState(string name, string newName) {
            if (Transitions.Contains(name)) {
                Transitions.Remove(name);
                Transitions.Add(newName);
            }
        }
    }
}