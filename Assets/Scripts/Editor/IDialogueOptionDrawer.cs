using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;
using System.Linq;
using ASimpleRoguelike.Dialogue;

namespace ASimpleRoguelike.Editor {
    [CustomPropertyDrawer(typeof(IDialogueOption))]
    public class IDialogueOptionDrawer : PropertyDrawer {
        private static List<Type> dialogueOptionTypes;
        private static string[] dialogueOptionTypeNames;

        static IDialogueOptionDrawer() {
            if (dialogueOptionTypes == null) {
                dialogueOptionTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => typeof(IDialogueOption).IsAssignableFrom(p) && p.IsClass && !p.IsAbstract)
                    .ToList();

                dialogueOptionTypeNames = dialogueOptionTypes.Select(t => t.Name).ToArray();
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            Rect typeDropdownRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            
            int selectedIndex = -1;
            if (property.managedReferenceValue != null) {
                selectedIndex = dialogueOptionTypes.FindIndex(t => t == property.managedReferenceValue.GetType());
            }

            int newSelectedIndex = EditorGUI.Popup(typeDropdownRect, label.text, selectedIndex, dialogueOptionTypeNames);

            if (newSelectedIndex != selectedIndex) {
                Type newType = dialogueOptionTypes[newSelectedIndex];
                property.managedReferenceValue = Activator.CreateInstance(newType);
            }

            if (property.managedReferenceValue != null) {
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.indentLevel--;
                EditorGUI.PropertyField(position, property, true);
                EditorGUI.indentLevel++;
            }
        }
    
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            float height = EditorGUIUtility.singleLineHeight;
            if (property.managedReferenceValue != null) {
                height += EditorGUI.GetPropertyHeight(property, true);
            }
            return height;
        }
    }
}