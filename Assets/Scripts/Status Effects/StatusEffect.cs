using UnityEngine;

namespace ASimpleRoguelike.StatusEffects {
    public class StatusEffect {
        public StatusEffectData statusEffect;
        public float timer;
        public float timerDamage;
        public GameObject spawned;

        public StatusEffect(StatusEffectData statusEffectData) {
            statusEffect = statusEffectData;
            timer = statusEffect.time;
            timerDamage = statusEffectData.timerDamageTime;
        }

        public void Apply(Entity.Entity entity) {
            if (statusEffect.toSpawn != null) {
                spawned = GameObject.Instantiate(statusEffect.toSpawn, entity.transform);
            }
        }

        public bool Update(float deltaTime, Entity.Entity entity) {
            timer -= deltaTime;
            timerDamage -= deltaTime;

            if (timerDamage < 0) {
                timerDamage = statusEffect.timerDamageTime;
                entity.DirectDamage((int)statusEffect.damage);
            }

            if (timer < 0) {
                GameObject.Destroy(spawned);
                return true;
            }
            return false;
        }
    }
}