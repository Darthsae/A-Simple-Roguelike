using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace ASimpleRoguelike.Editor {
    public class StateMachineEditorWindow : EditorWindow {
        [MenuItem("Tools/StateMachineEditorWindow")]
        public static void ShowEditorWindow() {
            GetWindow<StateMachineEditorWindow>();
        }

        public void CreateGUI() {
            TwoPaneSplitView splitView = new(0, 250, TwoPaneSplitViewOrientation.Horizontal);

            rootVisualElement.Add(splitView);

            VisualElement leftPane = new();
            VisualElement rightPane = new();

            splitView.Add(leftPane);
            splitView.Add(rightPane);
        }
    }
}