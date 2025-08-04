using System.Collections.Generic;
using ASimpleRoguelike.StatusEffects;
using ASimpleRoguelike.Util;
using UnityEngine;

namespace ASimpleRoguelike.Entity {
    public class Neutral : Entity {
        public static List<Neutral> neutrals = new();

        public bool marked = false;

        public static void DespawnAll() {
            MethodQueue queue = new();
            foreach (Neutral neutral in neutrals) {
                queue.Enqueue(neutral.Despawn);
            }
            queue.InvokeAll();
        }

        public NeutralAIType AIType = NeutralAIType.Stand;
        public float rotationOffset = -90f;
        public int xp = 10;

        public float timerer;

        public float dropChance = 0.05f;
        public Transform target;
        private Transform player;
        public WeightedEntry<PickupType>[] pickups;

        public bool dropPickup = true;
        public bool dropXP = true;

        private void Start() {
            rb = GetComponent<Rigidbody2D>();
            player = GameObject.FindGameObjectWithTag("Player").transform; // Assuming the player has the "Player" tag
            health.OnHealthZero += Die;

            neutrals.Add(this);
        }

        private void OnDestroy() {
            neutrals.Remove(this);
        }

        void OnDespawnTrigger() {
            Despawn();
        }
        
        public override void UpdateAI() {
            base.UpdateAI();

            timerer += Time.deltaTime;

            switch (AIType) {
                default:
                    break;
            }
        }

        void OnTriggerEnter2D(Collider2D other) {
            //Debug.Log(other.gameObject.name + "." + other.gameObject.tag);
            if (GlobalGameData.isPaused) return;
            
            CallDamage(other);
        }

        public void CallDamage(Collider2D other) {
            if (Time.time < nextInvulnerabilityTime) {
                return;
            }

            //Debug.Log(other.gameObject.tag + "." + (other.gameObject.TryGetComponent<Projectile>(out var a) ? a.owner : "None"));

            if (other.gameObject.CompareTag("Projectile") && other.gameObject.TryGetComponent<Projectile>(out var projectile) && projectile.owner == Owner.Player) {
                projectile.Hit();
                
                foreach (StatusEffectData effect in projectile.effects) {
                    AddStatusEffect(effect);
                }
                DirectDamage((int)-projectile.damage, false);
            }
        }

        public void Despawn() {
            dropPickup = false;
            dropXP = false;
            health.ForceKill();
        }

        private void Die() {
            if (player == null) {
                player = GameObject.FindGameObjectWithTag("Player").transform; // Assuming the player has the "Player" tag
            }

            if (dropPickup && Random.Range(0, 1f) < dropChance) {
                GameObject clone = Instantiate(GameObject.FindGameObjectWithTag("GlobalHolder").GetComponent<GlobalGameData>().pickup, transform.position, Quaternion.identity);
                clone.transform.parent = null;

                float totalChoice = 0f;
                for (int i = 0; i < pickups.Length; i++) {
                    totalChoice += pickups[i].Weight;
                }

                float randomChoice = Random.Range(0f, totalChoice);

                for (int i = 0; i < pickups.Length; i++) {
                    if (randomChoice < pickups[i].Weight) {
                        clone.GetComponent<Pickup>().Init(pickups[i].Item);
                        break;
                    } else {
                        randomChoice -= pickups[i].Weight;
                    }
                }
            }

            if (dropXP) {
                player.GetComponent<Player>().ChangeXP(xp);
            }

            neutrals.Remove(this);
            if (marked) {
                return;
            }
            marked = true;
            Destroy(gameObject);
        }
    }

    [System.Serializable]
    public enum NeutralAIType {
        Stand
    }
}