using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ASimpleRoguelike.Dialogue {
    [Serializable]
    public struct SpriteContext {
        public string context;
        public Sprite sprite;
    }

    [CreateAssetMenu(menuName = "A Simple Roguelike/DialoguePortrait")]
    public class DialoguePortrait : ScriptableObject {
        public List<SpriteContext> contexts;

        public Sprite defaultTexture;

        public void Apply(ref Image a_image, string a_context) {
            if (contexts.FindIndex(n => n.context == a_context) != -1) {
                a_image.sprite = contexts.Find(n => n.context == a_context).sprite;
            } else {
                a_image.sprite = defaultTexture;
            }
        }
    }
}