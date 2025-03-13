using System;
using UnityEngine;
using ASimpleRoguelike.Entity;

namespace ASimpleRoguelike {
    public class HarmingArea : MonoBehaviour
    {
        Action<Collider2D> action;

        public int damage = 1;
        public bool hurtPlayer = true;
        public bool hurtEnemy = false;

        void Start()
        {
            if (GetComponent<RingCollider>() != null)
            {
                if (hurtPlayer) {
                    GetComponent<RingCollider>().onCollide += (other) => {
                        if (other.gameObject.GetComponent<Player>() != null) {
                            other.gameObject.GetComponent<Player>().DirectDamage(-damage);
                        }
                    };
                }
                if (hurtEnemy) {
                    GetComponent<RingCollider>().onCollide += (other) => {
                        if (other.gameObject.GetComponent<Enemy>() != null) {
                            other.gameObject.GetComponent<Enemy>().DirectDamage(-damage);
                        }
                    };
                }
            }
            else {
                if (hurtPlayer) {
                    action += (other) => {
                        if (other.gameObject.GetComponent<Player>() != null) {
                            other.gameObject.GetComponent<Player>().DirectDamage(-damage);
                        }
                    };
                }
                if (hurtEnemy) {
                    action += (other) => {
                        if (other.gameObject.GetComponent<Enemy>() != null) {
                            other.gameObject.GetComponent<Enemy>().DirectDamage(-damage);
                        }
                    };
                }
            }
        }

        void OnTriggerStay2D(Collider2D other)
        {
            action?.Invoke(other);
        }
    }
}