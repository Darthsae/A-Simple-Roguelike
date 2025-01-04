using System;
using UnityEngine;
using ASimpleRoguelike.Entity;

namespace ASimpleRoguelike {
    public class HarmingArea : MonoBehaviour
    {
        Action<Collider2D> action;

        public int damage = 1;

        public Owner owner = Owner.Enemy;

        void Start()
        {
            if (GetComponent<RingCollider>() != null)
            {
                if (owner == Owner.Enemy) {
                    GetComponent<RingCollider>().onCollide += (other) => {
                        if (other.gameObject.GetComponent<Player>() != null) {
                            other.gameObject.GetComponent<Player>().DirectDamage(-damage);
                        }
                    };
                }
                else if (owner == Owner.Player) {
                    GetComponent<RingCollider>().onCollide += (other) => {
                        if (other.gameObject.GetComponent<Enemy>() != null) {
                            other.gameObject.GetComponent<Enemy>().DirectDamage(-damage);
                        }
                    };
                }
            }
            else {
                if (owner == Owner.Enemy) {
                    action += (other) => {
                        if (other.gameObject.GetComponent<Player>() != null) {
                            other.gameObject.GetComponent<Player>().DirectDamage(-damage);
                        }
                    };
                }
                else if (owner == Owner.Player) {
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