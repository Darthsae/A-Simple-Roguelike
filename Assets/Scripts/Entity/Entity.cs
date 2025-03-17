using System.Collections.Generic;
using System.Linq;
using ASimpleRoguelike.StatusEffects;
using UnityEngine;

namespace ASimpleRoguelike.Entity {
    public class Entity : MonoBehaviour {
        public Health health;
        public AudioSource hurtSound;
        protected Rigidbody2D rb;

        public FactionData faction;

        public List<StatusEffect> effects = new();

        #region Invulnerability
        public float invulnerabilityDuration = 0.5f;
        protected float nextInvulnerabilityTime = 0f;
        #endregion

        public virtual void UpdateOther() {
            
        }

        public void Update() {
            UpdateOther();
            if (GlobalGameData.isPaused) {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0;
                return;
            } else {
                UpdateAI();
            }
        }

        public virtual void UpdateAI() {
            List<int> toNuke = new();

            for (int i = 0; i < effects.Count; ++i) {
                if (effects[i].Update(Time.deltaTime, this)) {
                    toNuke.Add(i);
                }
            }

            toNuke.Reverse();

            for (int i = 0; i < toNuke.Count; ++i) {
                effects.RemoveAt(toNuke[i]);
            }
        }

        public void AddStatusEffect(StatusEffectData effect) {
            effects.Add(new StatusEffect(effect));
            effects.Last().Apply(this);
        }

        public void DirectDamage(int amount, bool ignoreInvulnerability = false) {
            if (!ignoreInvulnerability && Time.time < nextInvulnerabilityTime) {
                return;
            }

            //Debug.Log("Direct damage: " + amount + " ignoreInvulnerability: " + ignoreInvulnerability);

            health.ChangeHealth(amount);

            if (!ignoreInvulnerability) {
                nextInvulnerabilityTime = Time.time + invulnerabilityDuration;
            }

            if (hurtSound != null) {
                hurtSound.Play();
            }
        }
    }
}