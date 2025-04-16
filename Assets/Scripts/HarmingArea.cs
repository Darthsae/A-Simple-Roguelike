using System;
using UnityEngine;
using ASimpleRoguelike.Entity;
using System.Collections.Generic;
using ASimpleRoguelike.StatusEffects;

namespace ASimpleRoguelike {
    public class HarmingArea : MonoBehaviour {
        Action<Collider2D> action;

        public int damage = 1;
        public bool hurtPlayer = true;
        public bool hurtEnemy = false;

        public List<StatusEffectData> effectDatas = new();
        public GameObject onHitEffect;

        void Start() {
            if (GetComponent<RingCollider>() != null) {
                if (hurtPlayer) {
                    GetComponent<RingCollider>().onCollide += (other) => {
                        if (other.gameObject.GetComponent<Player>() != null) {
                            other.gameObject.GetComponent<Player>().DirectDamage(-damage);
                            foreach (StatusEffectData effect in effectDatas) {
                                other.gameObject.GetComponent<Player>().AddStatusEffect(effect);
                            }
                            if (onHitEffect != null) {
                                Instantiate(onHitEffect, other.gameObject.transform.position, Quaternion.identity);
                            }
                        }
                    };
                }
                if (hurtEnemy) {
                    GetComponent<RingCollider>().onCollide += (other) => {
                        if (other.gameObject.GetComponent<Enemy>() != null) {
                            other.gameObject.GetComponent<Enemy>().DirectDamage(-damage);
                            foreach (StatusEffectData effect in effectDatas) {
                                other.gameObject.GetComponent<Enemy>().AddStatusEffect(effect);
                            }
                            if (onHitEffect != null) {
                                Instantiate(onHitEffect, other.gameObject.transform.position, Quaternion.identity);
                            }
                        }
                    };
                }
            } else {
                if (hurtPlayer) {
                    action += (other) => {
                        if (other.gameObject.GetComponent<Player>() != null) {
                            other.gameObject.GetComponent<Player>().DirectDamage(-damage);
                            foreach (StatusEffectData effect in effectDatas) {
                                other.gameObject.GetComponent<Player>().AddStatusEffect(effect);
                            }
                            if (onHitEffect != null) {
                                Instantiate(onHitEffect, other.gameObject.transform.position, Quaternion.identity);
                            }
                        }
                    };
                }
                if (hurtEnemy) {
                    action += (other) => {
                        if (other.gameObject.GetComponent<Enemy>() != null) {
                            other.gameObject.GetComponent<Enemy>().DirectDamage(-damage);
                            foreach (StatusEffectData effect in effectDatas) {
                                other.gameObject.GetComponent<Enemy>().AddStatusEffect(effect);
                            }
                            if (onHitEffect != null) {
                                Instantiate(onHitEffect, other.gameObject.transform.position, Quaternion.identity);
                            }
                        }
                    };
                }
            }
        }

        void OnTriggerStay2D(Collider2D other) {
            action?.Invoke(other);
        }
    }
}