using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ASimpleRoguelike {
    public class InstancedGlobal : MonoBehaviour {
        public Image spriteRenderer;

        public TMP_Text tooltipText;
        public GameObject tooltip;

        void Awake() {
            GlobalGameData.spriteRenderer = spriteRenderer;
            GlobalGameData.tooltipText = tooltipText;
            GlobalGameData.tooltip = tooltip;
        }
    }
}