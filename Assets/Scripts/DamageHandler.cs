using System;
using UnityEngine;

namespace ASimpleRoguelike {
    public class DamageHandler : MonoBehaviour {
        public Action<int> OnDamage;
        public void Damage(int amount) => OnDamage?.Invoke(amount);

        public void OnTriggerEnter2D(Collider2D other) {
            
        }
    }
}