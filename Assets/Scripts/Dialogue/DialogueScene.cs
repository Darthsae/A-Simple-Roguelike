using System;
using System.Collections.Generic;
using UnityEngine;

namespace ASimpleRoguelike.Dialogue {
    [CreateAssetMenu(menuName = "A Simple Roguelike/DialogueScene")]
    public class DialogueScene : ScriptableObject {
        public List<DialogueLine> lines;

        [SerializeReference]
        public IDialogueOption response = new EndDialogueScene();
    }

    [Serializable]
    public struct DialogueLine {
        [Header("Speech")]
        public float speechSpeed;
        public string speechText;
        public DialoguePortrait portrait;
        public string context;

        [Header("Responses")]
        [SerializeReference]
        public List<IDialogueOption> responses;
        public float responseTime;
        public bool unskipable;
        public bool pauses;
        public bool pauseMusic;
    }

    [Serializable]
    public abstract class IDialogueOption {
        public string name;
        public abstract void Call(GlobalGameData a_globalGameData);
    }

    [Serializable]
    public class ChangeDialogueScene : IDialogueOption {
        public DialogueScene nextScene;

        public override void Call(GlobalGameData a_globalGameData) {
            DialogueManager.Instance.ChangeScene(nextScene);
        }
    }
    
    [Serializable]
    public class EndDialogueScene : IDialogueOption {
        public override void Call(GlobalGameData a_globalGameData) {
            DialogueManager.Instance.EndScene();
        }
    }
}