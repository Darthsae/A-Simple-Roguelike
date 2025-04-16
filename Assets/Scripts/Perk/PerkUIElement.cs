using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ASimpleRoguelike.Perk {
    public class PerkUIElement : MonoBehaviour {
        public PerkWithLevel perkWithLevel;
        public TMP_Text nameText;
        public TMP_Text descriptionText;
        public TMP_Text levelText;
        public Image icon;

        public void Init(PerkWithLevel perkWithLevel) {
            this.perkWithLevel = perkWithLevel;
            
            nameText.text = perkWithLevel.perk.name;
            descriptionText.text = perkWithLevel.perk.description;
            levelText.text = perkWithLevel.level.ToString();
            icon.sprite = perkWithLevel.perk.sprite;
        }
    }
}