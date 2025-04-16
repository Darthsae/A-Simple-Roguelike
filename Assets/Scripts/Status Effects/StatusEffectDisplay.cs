using ASimpleRoguelike.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ASimpleRoguelike.StatusEffects {
    public class StatusEffectDisplay : UIBehaviour {
        public PackedDisplay display;
        public GameObject icon;

        public StatusEffectIcon Add(StatusEffect statusEffect) {
            GameObject gameObject = Instantiate(icon, display.gameObject.transform);
            display.AddItem(gameObject);
            gameObject.GetComponent<StatusEffectIcon>().Init(this, statusEffect);
            return gameObject.GetComponent<StatusEffectIcon>();
        }

        public void Remove(GameObject gameObject) {
            display.RemoveItem(gameObject);
        }
    }
}
