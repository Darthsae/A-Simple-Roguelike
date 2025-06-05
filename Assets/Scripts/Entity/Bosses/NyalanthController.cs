using System;
using UnityEngine;

namespace ASimpleRoguelike.Entity.Bosses {
    public class NyalanthController : Boss {
        #region Locamotion Info
        private float time = 0f;
        public float speed = 10f;
        public float turningSpeed = 180f;

        public bool brainDead = false;
        #endregion

        #region CURVES
        public AnimationCurve chargeCurve;
        public AnimationCurve attackCurve;
        #endregion

        #region Attack Info
        public float attackDamage = 15f;

        public float chargeAttackDamage = 30f;
        public float chargeSpeed = 10f;
        public float chargeAttackTime = 2.0f;
        public float chargeCooldownTime = 1.0f;
        private float currentChargeTime = 0f;
        private bool isCharging = false;
        private int charged = 0;
        public int maxCharges = 3;

        public float attackDelayTime = 0.5f;
        private float nextAttackTime = 0f;

        public bool shouldMove = true;

        public int shootBeforeCharge = 3;
        private int internalTimer = 0;

        public GameObject shockwave;
        public float shockwaveCooldownTime = 0.5f;
        #endregion
        
        #region Sprite Info
        public SpriteRenderer bodySprite;
        public SpriteRenderer backGiantFlipper;
        public SpriteRenderer leftGiantFlipper;
        public SpriteRenderer rightGiantFlipper;
        public SpriteRenderer leftTentacleBaseSprite;
        public SpriteRenderer rightTentacleBaseSprite;
        public SpriteRenderer leftTentacleSegmentSprite;
        public SpriteRenderer rightTentacleSegmentSprite;
        public SpriteRenderer leftTentacleEndSprite;
        public SpriteRenderer rightTentacleEndSprite;
        public SpriteRenderer leftTentacleTipSprite;
        public SpriteRenderer rightTentacleTipSprite;
        public SpriteRenderer leftFlipper;
        public SpriteRenderer rightFlipper;
        #endregion

        #region Audio
        public AudioSource roarSound;
        public AudioSource chargeSound;
        #endregion

        #region Nyalanth Data
        public NyalanthAIState AIState;
        public NyalanthType type;
        #endregion

        void Start() {
            rb = GetComponent<Rigidbody2D>();
            player = GameObject.FindGameObjectWithTag("Player").transform; // Assuming the player has the "Player" tag
            
            AIState = NyalanthAIState.Follow;

            type = NyalanthType.Lapis;

            health.SetMaxHealth(NyalanthMaxHealth(type));
            health.OnHealthZero += Die;
        
            xp = NyalanthXP(type);

            if (roarSound != null)
                roarSound.Play();
        }

        public override void UpdateAI() {
            base.UpdateAI();
            
            rb.angularVelocity = 0;

            switch (AIState) {
                case NyalanthAIState.Follow:
                    Follow();
                    break;
                case NyalanthAIState.Attack:
                    Attack();
                    break;
                case NyalanthAIState.Charge:
                    Charge();
                    break;
                case NyalanthAIState.Slam:
                    Slam();
                    break;
            }
        }

        public void Die() {
            Destroy(gameObject);
        }

        #region AI States
        public void Follow() {
            if (player == null) return;

            time += 0.05f;

            Vector3 targetPos = player.position + new Vector3(MathF.Cos(time / 25f), MathF.Sin(time / 25f)) * 8.5f  + new Vector3(MathF.Cos(time / 15f), MathF.Sin(time / 15f)) * 2.5f;

            float idealAngle = Util.AngleToPlayer(transform, player);

            rb.rotation = Mathf.MoveTowardsAngle(transform.rotation.eulerAngles.z, idealAngle, turningSpeed * Time.deltaTime);

            rb.MovePosition(Vector2.MoveTowards(transform.position, targetPos, speed * 0.05f));

            if (nextAttackTime > 0) {
                nextAttackTime -= Time.deltaTime;
            } else {
                internalTimer++;
                if (internalTimer >= shootBeforeCharge) {
                    AIState = NyalanthAIState.Charge;
                    internalTimer = 0;
                } else {
                    nextAttackTime = attackDelayTime;
                    AIState = NyalanthAIState.Attack;
                }
            }
        }

        public void Attack() {
            if (player == null) return;

            if (shouldMove) {
                float idealAngle = Util.AngleToPlayer(transform, player);

                rb.rotation = Mathf.MoveTowardsAngle(transform.rotation.eulerAngles.z, idealAngle, turningSpeed * Time.deltaTime);
            }

            if (nextAttackTime > 0) {
                nextAttackTime -= Time.deltaTime;
            } else {
                nextAttackTime = attackDelayTime;

                AIState = (health.health >= health.maxHealth * 0.5) ? NyalanthAIState.Follow : NyalanthAIState.Slam;
            }
        }

        public void Charge() {
            if (player == null) return;

            if (shouldMove) { 
                if (!isCharging) {
                    float idealAngle = Util.AngleToPlayer(transform, player);

                    if (currentChargeTime <= chargeAttackTime * chargeCurve.Evaluate((float) health.health / health.maxHealth) * 0.8f) {
                        rb.rotation = Mathf.MoveTowardsAngle(transform.rotation.eulerAngles.z, idealAngle, 15f);
                    } else {
                        rb.velocity = (Vector2)(Time.deltaTime * 3.5f * -transform.right);
                    }
                    
                    currentChargeTime += Time.deltaTime;
                    if (currentChargeTime >= chargeAttackTime * chargeCurve.Evaluate((float) health.health / health.maxHealth)) {
                        currentChargeTime = 0;
                        if (charged < maxCharges) {
                            charged++;
                            if (chargeSound != null) {
                                chargeSound.Play();
                            }
                            isCharging = true;
                        } else {
                            nextAttackTime = attackDelayTime;
                            currentChargeTime = 0;
                            charged = 0;
                            AIState = NyalanthAIState.Follow;
                        }
                    }
                } else {
                    rb.velocity = (Vector2)(chargeSpeed * Time.deltaTime * -transform.up);

                    currentChargeTime += Time.deltaTime;

                    if (currentChargeTime >= chargeCooldownTime * chargeCurve.Evaluate((float) health.health / health.maxHealth)) {
                        currentChargeTime = 0;
                        isCharging = false;
                        rb.velocity = Vector2.zero;
                    }
                }
            }
        }
        
        void Slam() {
            nextAttackTime = shockwaveCooldownTime;

            Instantiate(shockwave, transform.position, transform.rotation);

            if (roarSound != null) {
                roarSound.Play();
            }

            AIState = NyalanthAIState.Follow;
        }
        #endregion

        void OnTriggerEnter2D(Collider2D other) {
            if (GlobalGameData.isPaused) return;
            
            CallDamage(other);
        }

        void OnCollisionEnter2D(Collision2D other) {
            if (GlobalGameData.isPaused) return;
            
            if (other.gameObject.TryGetComponent<Player>(out var player)) {
                player.DirectDamage((int)(isCharging ? chargeAttackDamage : attackDamage));
                //Get rigidbody of player and add force to it
                other.rigidbody.GetComponent<Rigidbody2D>().AddForce((transform.position - player.transform.position).normalized * 10f, ForceMode2D.Impulse);
            }
        }

        public void CallDamage(Collider2D other) {
            if (Time.time < nextInvulnerabilityTime) {
                return;
            }

            if (other.gameObject.CompareTag("Projectile") && other.gameObject.TryGetComponent<Projectile>(out var projectile) && projectile.owner == Owner.Player) {
                projectile.Hit();
                health.ChangeHealth((int)-projectile.damage);

                nextInvulnerabilityTime = Time.time + invulnerabilityDuration;

                if (hurtSound != null) {
                    hurtSound.Play();
                }
            }
        }


        #region Static Nyalanth Type Functions
        public static int NyalanthMaxHealth(NyalanthType type) {
            return type switch {
                NyalanthType.Lapis => 7500,
                _ => 1000,
            };
        }

        public static int NyalanthXP(NyalanthType type) {
            return type switch {
                NyalanthType.Lapis => 250,
                _ => 150,
            };
        }
        #endregion
    }

    [Serializable]
    public enum NyalanthAIState {
        Follow,
        Attack,
        Charge,
        Slam,
        Die
    }

    [Serializable]
    public enum NyalanthType {
        Ignis, // Fire
        Lapis, // Stone
        Glacies, // Ice
        Aquae // Water
    }
}