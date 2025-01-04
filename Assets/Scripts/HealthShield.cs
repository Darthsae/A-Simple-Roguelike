using UnityEngine;
using ASimpleRoguelike.Entity;

namespace ASimpleRoguelike {
    public class HealthShield : MonoBehaviour
    {
        public Health health;
        public float invulnerabilityDuration = 0.5f;
        public float dropChance = 0.05f;
        private float nextInvulnerabilityTime = 0f;
        public int damage = 10;

        void OnTriggerEnter2D(Collider2D other) {
            if (GlobalGameData.isPaused) return;
            
            CallDamage(other);
        }

        public void CallDamage(Collider2D other) {
            if (Time.time < nextInvulnerabilityTime) {
                return;
            }

            if (other.gameObject.CompareTag("Enemy") && other.gameObject.TryGetComponent<Enemy>(out var enemy)) {
                enemy.DirectDamage(-damage, true);
                DirectDamage(-1, true);
            }
        }

        public void DirectDamage(int amount, bool ignoreInvulnerability = false) {
            if (!ignoreInvulnerability && Time.time < nextInvulnerabilityTime) {
                return;
            }

            //Debug.Log("Direct damage: " + amount + " ignoreInvulnerability: " + ignoreInvulnerability);

            health.ChangeHealth(amount);

            if (!ignoreInvulnerability) 
                nextInvulnerabilityTime = Time.time + invulnerabilityDuration;
        }
    }
}