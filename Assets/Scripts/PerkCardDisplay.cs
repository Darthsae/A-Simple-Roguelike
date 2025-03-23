using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ASimpleRoguelike {
    public class PerkCardDisplay : MonoBehaviour {
        public PerkManager perkManager;

        public PerkData perkData;
        public int level;

        public TMP_Text nameText;
        public TMP_Text descriptionText;
        public TMP_Text levelText;
        public Image icon;

        public void SetData(PerkData perkData, int level) {
            this.perkData = perkData;
            this.level = level;

            nameText.text = perkData.name;
            descriptionText.text = perkData.description;
            levelText.text = level.ToString();
            icon.sprite = perkData.sprite;
        }

        public void OnClick() {
            perkManager.SelectedPerk(perkData);
        }
    }
}