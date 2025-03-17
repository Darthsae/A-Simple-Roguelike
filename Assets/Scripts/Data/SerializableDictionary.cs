using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ASimpleRoguelike.Data {
    [Serializable]
    public class SerializableDictionary<K, V> : ISerializationCallbackReceiver {
        public Dictionary<K, V> dictionary;
        public List<K> keys;
        public List<V> values;

        public V this[K key] {
            get => dictionary[key];
            set => dictionary[key] = value;
        }

        public void OnBeforeSerialize() {
            foreach (KeyValuePair<K, V> pair in dictionary) {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        public void OnAfterDeserialize() {
            for (int i = 0; i < keys.Count; i++) {
                dictionary[keys[i]] = values[i];
            }

            keys.Clear();
            values.Clear();
        }
    }

    [CustomPropertyDrawer(typeof(SerializableDictionary<,>))]
    public class SerializableDictionaryEditor : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);
            property.isExpanded = EditorGUI.Foldout(new(position.x, position.y, position.width, 10), property.isExpanded, GUIContent.none);
            
            if (property.isExpanded) {
                
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return base.GetPropertyHeight(property, label);
        }
    }
}