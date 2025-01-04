using UnityEngine;
using ASimpleRoguelike.Entity;

namespace ASimpleRoguelike {
    [RequireComponent(typeof(Enemy), typeof(Health))]
    public class KillAfterAttacks : MonoBehaviour {
        public int maxAttacks = 1;
        private int attacks = 0;

        void Start() {
            GetComponent<Enemy>().OnAttack += () => {
                attacks++;
                if (attacks >= maxAttacks) {
                    GetComponent<Enemy>().Despawn();
                }
            };
        }
    }
}