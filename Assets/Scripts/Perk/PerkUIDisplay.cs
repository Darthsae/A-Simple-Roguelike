using ASimpleRoguelike.UI;
using UnityEngine;

namespace ASimpleRoguelike.Perk {
    public class PerkUIDisplay : MonoBehaviour {
        public PackedDisplay display;
        public PerkManager perkManager;
        public GameObject template;
        public GameObject content;

        public void Open() {
            display.Clear();
            foreach (PerkWithLevel perk in perkManager.unlockedPerks) {
                GameObject gameObject = Instantiate(template, content.transform);
                display.AddItemNoCalculate(gameObject);
                gameObject.GetComponent<PerkUIElement>().Init(perk);
            }
            display.Recalculate();
        }
    }
}