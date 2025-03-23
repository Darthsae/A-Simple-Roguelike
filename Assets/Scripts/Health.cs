using System;
using UnityEngine;

namespace ASimpleRoguelike {
    public delegate bool PreHealthZero(int health, out int newHealth);

    public class Health : MonoBehaviour {
        public int maxHealth = 100;
        public int health;
        public event Action OnHealthZero;
        public event Action<int, int> OnMaxHealthSet;
        public event Action<int> OnHealthChanged;
        public event PreHealthZero PreHealthZero;

        void Start() {
            health = maxHealth;
        }

        public void SetMaxHealth(int maxHealth) {
            int oldHealth = this.maxHealth;
            this.maxHealth = maxHealth;
            health = maxHealth;

            OnMaxHealthSet?.Invoke(oldHealth, maxHealth);
        }

        public void SetHealth(int health) {
            this.health = health;
            this.health = Math.Clamp(this.health, 0, maxHealth);

            if (this.health == 0) {
                if (PreHealthZero != null && !PreHealthZero(this.health, out int newHealth)) {
                    ChangeHealth(newHealth);
                    return;
                }
                
                OnHealthZero?.Invoke();
            }
        }

        public void ChangeHealth(int health) {
            this.health += health;

            this.health = Math.Clamp(this.health, 0, maxHealth);

            OnHealthChanged?.Invoke(health);

            if (this.health == 0) {
                if (PreHealthZero != null && !PreHealthZero(this.health, out int newHealth)) {
                    ChangeHealth(newHealth);
                    return;
                }
                
                OnHealthZero?.Invoke();
            }
        }

        public void ForceKill() {
            OnHealthZero?.Invoke();
        }
    }
}