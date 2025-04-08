using UnityEngine;

namespace ASimpleRoguelike.StatusEffects {
    public class StatusEffectIcon {
        public StatusEffect statusEffect;
        public Image icon;

        public void Init(StatusEffect statusEffect) {
            this.statusEffect = statusEffect;
            icon.sprite = statusEffect.statusEffect.sprite;
        }
    }
}

