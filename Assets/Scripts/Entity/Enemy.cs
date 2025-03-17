using System.Collections.Generic;
using ASimpleRoguelike.StatusEffects;
using UnityEngine;

namespace ASimpleRoguelike.Entity {
    public class Enemy : Entity {
        public static List<Enemy> enemies = new();

        public float laserRange = 5.0f;
        public float laserTime = 1.5f;
        public float laserCooldown = 2.5f;
        public float laserCharge = 0.75f;
        private float laserTimer = 0;
        public GameObject laserGameObject;
        public GameObject laserDetector;
        public AudioSource laserSound;
        public AudioSource laserChargeSound;
        public ParticleSystem laserParticles;

        public bool hasLaser = false;
        public bool chargingLaser = false;
        public bool canMoveWhileLaser = false;
        public bool usingLaser = false;
        public bool laserOnCooldown = false;

        public EnemyAIType AIType = EnemyAIType.Follow;
        public float rotationOffset = -90f;
        public int xp = 10;
        public float speed = 10f;

        public AnimationCurve modulateSpeedByDistance = AnimationCurve.Linear(0f, 1f, 1f, 1f);
        public AnimationCurve modulateZigAmountByDistance = AnimationCurve.Linear(0f, 1f, 1f, 1f);

        public float attackRange = 1f;
        public float attackDamage = 10f;
        public float attackRate = 1f;
        public float dropChance = 0.05f;
        private Transform player;
        private float nextAttackTime = 0f;
        public Spawner spawner;
        public WeightedEntry<PickupType>[] pickups;

        public bool dropPickup = true;
        public bool dropXP = true;

        public float circleDistance = 15f;
        public float delayBetweenAttacks = 7.5f;
        public float delayTimer = 0f;
        public bool isCircling = true;

        public bool isChargering = false;

        public float timerer;

        public float zigAmount = 5f;

        public event System.Action OnAttack;

        private void Start() {
            rb = GetComponent<Rigidbody2D>();
            player = GameObject.FindGameObjectWithTag("Player").transform; // Assuming the player has the "Player" tag
            health.OnHealthZero += Die;
            delayTimer = delayBetweenAttacks;

            enemies.Add(this);
        }

        private void OnDestroy() {
            enemies.Remove(this);
        }

        private void UpdateLaser() {
            if (chargingLaser) {
                laserTimer -= Time.deltaTime;
                if (laserTimer < 0.0f) {
                    laserGameObject.SetActive(true);
                    laserTimer = laserTime;
                    usingLaser = true;
                    chargingLaser = false;
                    laserSound.Play();
                }
            } else if (laserOnCooldown) {
                laserTimer -= Time.deltaTime;
                if (laserTimer < 0.0f) {
                    laserOnCooldown = false;
                }
            } else if (usingLaser) {
                laserTimer -= Time.deltaTime;
                if (laserTimer < 0.0f) {
                    laserGameObject.SetActive(false);
                    usingLaser = false;
                    laserOnCooldown = true;
                    laserTimer = laserCooldown;
                }
            } else if (laserDetector.GetComponent<BoxCollider2D>().IsTouching(player.GetComponent<Collider2D>())) {
                chargingLaser = true;
                laserTimer = laserCharge;
                laserChargeSound.Play();
                laserParticles.Play();
            }
        }

        public override void UpdateAI() {
            base.UpdateAI();

            timerer += Time.deltaTime;

            if (hasLaser) {
                UpdateLaser();
            }

            switch (AIType) {
                case EnemyAIType.Follow:
                    MoveTowardsPlayer();
                    if (IsPlayerInRange() && Time.time >= nextAttackTime) {
                        AttackPlayer();
                        nextAttackTime = Time.time + 1f / attackRate;
                    }
                    break;
                case EnemyAIType.ZigFollow:
                    MoveTowardsPlayerZigZag();
                    if (IsPlayerInRange() && Time.time >= nextAttackTime) {
                        AttackPlayer();
                        nextAttackTime = Time.time + 1f / attackRate;
                    }
                    break;
                case EnemyAIType.Circle:
                    if (isCircling) {
                        CirclePlayer();

                        if (delayTimer >= 0f) {
                            delayTimer -= Time.deltaTime;
                        } else {
                            delayTimer = delayBetweenAttacks;
                            isCircling = false;
                        }
                    } else {
                        MoveTowardsPlayer();
                        if (IsPlayerInRange()) {
                            AttackPlayer();
                            delayTimer = delayBetweenAttacks;
                            isCircling = true;
                        }

                        if (delayTimer >= 0f) {
                            delayTimer -= Time.deltaTime;
                        } else {
                            delayTimer = delayBetweenAttacks;
                            isCircling = true;
                        }
                    }
                    break;
                case EnemyAIType.EternalCircle:
                    CirclePlayer();
                    break;
            }
        }

        private void CirclePlayer() {
            if (hasLaser && usingLaser) {
                rb.velocity = Vector2.zero;
                return;
            }

            if (player != null) {
                Vector2 directionToPlayer = (player.position - transform.position).normalized;
                Vector2 perpendicularDirection = new Vector2(-directionToPlayer.y, directionToPlayer.x) * circleDistance; // Perpendicular to the direction to the player

                // Move around the player in a circular motion
                rb.velocity = (directionToPlayer + perpendicularDirection).normalized * speed * modulateSpeedByDistance.Evaluate(Vector2.Distance(transform.position, player.position)) * 1.5f;

                float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
                rb.rotation = angle + rotationOffset; // Adjust based on the enemy's default orientation
            }
        }

        private void MoveTowardsPlayer() {
            if (hasLaser && (usingLaser || chargingLaser)) {
                rb.velocity = Vector2.zero;
                return;
            }

            if (player != null) {
                Vector2 direction = (player.position - transform.position).normalized;
                rb.velocity = (AIType == EnemyAIType.Circle ? 1.75f : 1f) * speed * modulateSpeedByDistance.Evaluate(Vector2.Distance(transform.position, player.position)) * direction;

                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                rb.rotation = angle + rotationOffset; // Adjust based on the enemy's default orientation
            }
        }

        private void MoveTowardsPlayerZigZag() {
            if (hasLaser && (usingLaser || chargingLaser)) {
                rb.velocity = Vector2.zero;
                return;
            }

            if (player != null) {
                Vector2 direction = (player.position - transform.position).normalized;
                
                rb.velocity = (AIType == EnemyAIType.Circle ? 1.75f : 1f) * speed * modulateSpeedByDistance.Evaluate(Vector2.Distance(transform.position, player.position)) * direction;

                rb.velocity += zigAmount * modulateZigAmountByDistance.Evaluate(Vector2.Distance(transform.position, player.position)) * Mathf.Sin(timerer) * (Vector2)transform.up;

                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                rb.rotation = angle + rotationOffset; // Adjust based on the enemy's default orientation
            }
        }

        private bool IsPlayerInRange() {
            if (player != null && attackDamage * attackRate * attackRange != 0) {
                float distance = Vector2.Distance(transform.position, player.position);
                return distance <= attackRange;
            }
            return false;
        }

        private void AttackPlayer() {
            // Assuming the player has a Health component
            if (player.TryGetComponent<Health>(out var playerHealth)) {
                playerHealth.ChangeHealth((int)-attackDamage);
                OnAttack?.Invoke();
            }
        }

        void OnTriggerEnter2D(Collider2D other) {
            if (GlobalGameData.isPaused) return;
            
            CallDamage(other);
        }

        public void CallDamage(Collider2D other) {
            if (Time.time < nextInvulnerabilityTime) {
                return;
            }

            if (other.gameObject.CompareTag("Projectile") && other.gameObject.TryGetComponent<Projectile>(out var projectile) && projectile.owner == Owner.Player) {
                projectile.Hit();
                
                foreach (StatusEffectData effect in projectile.effects) {
                    AddStatusEffect(effect);
                }
                DirectDamage((int)-projectile.damage, true);
            }
        }

        public void Despawn() {
            dropPickup = false;
            dropXP = false;
            health.ForceKill();
        }

        private void Die() {
            // Handle enemy death (e.g., play death animation, drop loot, etc.)
            if (spawner != null) {
                spawner.spawnedCount -= 1;
            }

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

            Destroy(gameObject);
        }
    }

    [System.Serializable]
    public enum EnemyAIType {
        Follow,
        Circle,
        Harrass,
        Charge,
        EternalCircle,
        ZigFollow
    }

    [System.Serializable]
    public struct WeightedEntry<T> {
        public T Item;
        public float Weight;

        public WeightedEntry(T item, float weight) {
            Item = item;
            Weight = weight;
        }
    }

    public static class Util {
        #region Helper Functions
        public static Vector2 DirectionToPlayer(Transform transform, Transform player) {
            return (player.position - transform.position).normalized;
        }

        public static float AngleToPlayer(Transform transform, Transform player) {
            if (player == null) return 0;
            Vector2 direction = DirectionToPlayer(transform, player);
            return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;
        }

        public static float DistanceToPlayer(Transform transform, Transform player) {
            if (player == null) return 0;
            return Vector2.Distance(player.position, transform.position);
        }

        public static Vector3 RotateAroundOrigin(Vector3 origin, Vector3 rotate, float angle, Vector3 axis) {
            // Translate the point to the origin
            Vector3 direction = rotate - origin;

            // Create a quaternion representing the rotation around the given axis by the given angle
            Quaternion rotation = Quaternion.AngleAxis(angle, axis);

            // Rotate the direction vector
            Vector3 rotatedDirection = rotation * direction;

            // Translate the point back to its original position
            return rotatedDirection + origin;
        }

        public static float ActualAngle(Vector2 start, Vector2 end) {
            return Mathf.Atan2(end.y - start.y, end.x - start.x) * Mathf.Rad2Deg;
        }
        #endregion
    }
}