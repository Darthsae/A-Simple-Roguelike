using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ASimpleRoguelike.Data {
    [Serializable]
    public class SerializableDictionary<K, V> : ISerializationCallbackReceiver {
        public Dictionary<K, V> dictionary = new();
        public List<K> keys = new();
        public List<V> values = new();

        public V this[K key] {
            get => dictionary[key];
            set => dictionary[key] = value;
        }

        public void OnBeforeSerialize() {
            keys.Clear();
            values.Clear();
            foreach (KeyValuePair<K, V> pair in dictionary) {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        public void OnAfterDeserialize() {
            dictionary.Clear();

            for (int i = 0; i < keys.Count; i++) {
                if (keys[i] != null && !dictionary.ContainsKey(keys[i])) {
                    dictionary[keys[i]] = values[i];
                }
            }
        }
    }

    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SerializableDictionary<,>))]
    public class SerializableDictionaryEditor : PropertyDrawer {
        private const float DeleteButtonWidth = 20f;
        private const float FieldPadding = 5f;

        private static Dictionary<string, (SerializedObject, SerializedProperty, SerializedProperty)> newEntryObjects = new();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);
            
            SerializedProperty keysProperty = property.FindPropertyRelative("keys");
            SerializedProperty valuesProperty = property.FindPropertyRelative("values");
            string dictionaryPath = property.propertyPath;

            Rect foldoutRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label);

            if (property.isExpanded) {
                EditorGUI.indentLevel++;
                float entryHeight = EditorGUIUtility.singleLineHeight;
                float currentY = position.y + EditorGUIUtility.singleLineHeight;
                
                // Get or create temporary SerializedObject for new entry
                if (!newEntryObjects.ContainsKey(dictionaryPath)) {
                    var tempSO = new SerializedObject(property.serializedObject.targetObject);
                    keysProperty.InsertArrayElementAtIndex(0);
                    valuesProperty.InsertArrayElementAtIndex(0);
                    var tempKeyProp = keysProperty.GetArrayElementAtIndex(0).Copy();
                    var tempValueProp = valuesProperty.GetArrayElementAtIndex(0).Copy();
                    keysProperty.DeleteArrayElementAtIndex(0);
                    valuesProperty.DeleteArrayElementAtIndex(0);

                    newEntryObjects[dictionaryPath] = (tempSO, tempKeyProp, tempValueProp);
                }

                var (tempNewEntrySO, newKeyProp, newValueProp) = newEntryObjects[dictionaryPath];
                tempNewEntrySO.Update();

                // New entry header
                Rect newEntryHeaderRect = new(position.x, currentY, position.width, entryHeight);
                float keyHeaderWidth = (newEntryHeaderRect.width - DeleteButtonWidth - FieldPadding) / 2;
                float valueHeaderWidth = (newEntryHeaderRect.width - DeleteButtonWidth - FieldPadding) / 2;
                
                EditorGUI.LabelField(new(newEntryHeaderRect.x, newEntryHeaderRect.y, keyHeaderWidth, entryHeight), "New Key");
                EditorGUI.LabelField(new(newEntryHeaderRect.x + keyHeaderWidth + FieldPadding, newEntryHeaderRect.y, valueHeaderWidth, entryHeight), "New Value");
                currentY += entryHeight;

                // New entry fields
                Rect newEntryFieldsRect = new(position.x, currentY, position.width, entryHeight);
                Rect newKeyRect = new(newEntryFieldsRect.x, newEntryFieldsRect.y, keyHeaderWidth, entryHeight);
                Rect newValueRect = new(newEntryFieldsRect.x + keyHeaderWidth + FieldPadding, newEntryFieldsRect.y, valueHeaderWidth, entryHeight);
                Rect addButtonRect = new(newEntryFieldsRect.x + keyHeaderWidth + valueHeaderWidth + FieldPadding * 2, newEntryFieldsRect.y, DeleteButtonWidth, entryHeight);

                EditorGUI.PropertyField(newKeyRect, newKeyProp, GUIContent.none);
                EditorGUI.PropertyField(newValueRect, newValueProp, GUIContent.none);

                if (GUI.Button(addButtonRect, "+")) {
                    keysProperty.InsertArrayElementAtIndex(keysProperty.arraySize);
                    valuesProperty.InsertArrayElementAtIndex(valuesProperty.arraySize);
                    
                    SerializedProperty addedKeyProp = keysProperty.GetArrayElementAtIndex(keysProperty.arraySize - 1);
                    SerializedProperty addedValueProp = valuesProperty.GetArrayElementAtIndex(valuesProperty.arraySize - 1);
                    
                    addedKeyProp.serializedObject.CopyFromSerializedProperty(newKeyProp);
                    addedValueProp.serializedObject.CopyFromSerializedProperty(newValueProp);

                    SetDefaultValue(newKeyProp);
                    SetDefaultValue(newValueProp);
                }

                tempNewEntrySO.ApplyModifiedProperties();
                currentY += entryHeight + FieldPadding;

                // Existing entries section
                for (int i = 0; i < keysProperty.arraySize; i++) {
                    Rect rowRect = new(position.x, currentY, position.width, entryHeight);
                    float existingKeyWidth = (rowRect.width - DeleteButtonWidth) / 2;
                    float existingValueWidth = (rowRect.width - DeleteButtonWidth) / 2;

                    Rect keyRect = new(rowRect.x, rowRect.y, existingKeyWidth, entryHeight);
                    Rect valueRect = new(rowRect.x + existingKeyWidth, rowRect.y, existingValueWidth, entryHeight);
                    Rect deleteRect = new(rowRect.x + existingKeyWidth + existingValueWidth, rowRect.y, DeleteButtonWidth, entryHeight);
                    
                    EditorGUI.PropertyField(keyRect, keysProperty.GetArrayElementAtIndex(i), GUIContent.none);
                    EditorGUI.PropertyField(valueRect, valuesProperty.GetArrayElementAtIndex(i), GUIContent.none);

                    if (GUI.Button(deleteRect, "X")) {
                        keysProperty.DeleteArrayElementAtIndex(i);
                        valuesProperty.DeleteArrayElementAtIndex(i);
                        
                        property.serializedObject.ApplyModifiedProperties();
                        return;
                    }
                    currentY += entryHeight;
                }

                EditorGUI.indentLevel--;
            } else {
                if (newEntryObjects.ContainsKey(dictionaryPath)) {
                    newEntryObjects.Remove(dictionaryPath);
                }
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            SerializedProperty keysProperty = property.FindPropertyRelative("keys");
            float height = EditorGUIUtility.singleLineHeight;

            if (property.isExpanded) {
                height += keysProperty.arraySize * EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight * 2 + FieldPadding;
            }
            return height;
        }

        private void SetDefaultValue(SerializedProperty prop) {
            switch (prop.propertyType) {
                case SerializedPropertyType.Integer:
                case SerializedPropertyType.Float:
                    prop.floatValue = 0;
                    break;
                case SerializedPropertyType.String:
                    prop.stringValue = "";
                    break;
                case SerializedPropertyType.Boolean:
                    prop.boolValue = false;
                    break;
                case SerializedPropertyType.Color:
                    prop.colorValue = Color.black;
                    break;
                case SerializedPropertyType.ObjectReference:
                    prop.objectReferenceValue = null;
                    break;
                case SerializedPropertyType.Enum:
                    prop.enumValueIndex = 0;
                    break;
                default:
                    prop.managedReferenceValue = null;
                    break;
            }
        }
    }
    #endif
}