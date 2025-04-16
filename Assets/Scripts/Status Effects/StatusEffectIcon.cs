using UnityEngine;
using UnityEngine.UI;

namespace ASimpleRoguelike.StatusEffects {
    public class StatusEffectIcon : MonoBehaviour {
        public StatusEffectDisplay display;
        public StatusEffect statusEffect;
        public Image icon;

        public void Init(StatusEffectDisplay display, StatusEffect statusEffect) {
            this.display = display;
            this.statusEffect = statusEffect;
            icon.sprite = statusEffect.statusEffect.sprite;
        }

        public void Finished() {
            display.Remove(gameObject);
            Destroy(gameObject);
        }
    }
}

