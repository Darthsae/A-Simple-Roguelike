using UnityEngine;

namespace ASimpleRoguelike.Entity {
    public class Entity : MonoBehaviour
    {
        public Health health;
        public AudioSource hurtSound;
        protected Rigidbody2D rb;

        public FactionData faction;

        #region Invulnerability
        public float invulnerabilityDuration = 0.5f;
        protected float nextInvulnerabilityTime = 0f;
        #endregion

        public void DirectDamage(int amount, bool ignoreInvulnerability = false) {
            if (!ignoreInvulnerability && Time.time < nextInvulnerabilityTime) {
                return;
            }

            //Debug.Log("Direct damage: " + amount + " ignoreInvulnerability: " + ignoreInvulnerability);

            health.ChangeHealth(amount);

            if (!ignoreInvulnerability) 
                nextInvulnerabilityTime = Time.time + invulnerabilityDuration;

            if (hurtSound != null) {
                hurtSound.Play();
            }
        }
    }
}