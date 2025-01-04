using UnityEngine;

namespace ASimpleRoguelike.Entity.Bosses {
    public class LichController : Entity
    {
        public int xp = 10;
        public Transform player;
        
        public int phase = 0;

        #region Locamotion Info
        public float speed = 2.5f;
        public float turningSpeed = 180f;
        public float rotationOffset = 0f;
        public float circleDistance = 12f;

        public bool brainDead = false;
        #endregion

        #region Magic Circles
        public GameObject summoningCircle;
        #endregion 

        #region Attack Info
        public float attackDamage = 15f;

        public int homingProjectileCount = 4;
        public int fleshLumpCount = 2;

        public GameObject homingProjectile;
        public GameObject fleshLump;
        #endregion

        #region Timer Info
        public float maxTeleportTimer = 2.5f;
        public float teleportTimer = 0f;

        public float maxSummonTimer = 1.5f;
        public float maxHomingTimer = 1.0f;
        public float attackTimer = 0f;

        public float maxCircleTimer = 4f;
        public float circleTimer = 0f;
        #endregion

        #region Counters
        public int maxHomingCounter = 3;
        public int maxSummonCounter = 1;
        public int counter = 0;
        #endregion
        
        #region Sprite Info
        public SpriteRenderer bodySprite;
        #endregion

        #region Audio
        public AudioSource roarSound;
        public AudioSource chargeSound;
        #endregion

        #region Lich Data
        public LichAIState AIState;
        public LichType type;
        #endregion

        void Start() {
            rb = GetComponent<Rigidbody2D>();
            player = GameObject.FindGameObjectWithTag("Player").transform; // Assuming the player has the "Player" tag
            
            AIState = LichAIState.Follow;

            type = LichType.Aserik;

            health.SetMaxHealth(LichMaxHealth(type));
            health.OnHealthZero += Die;
        
            xp = LichXP(type);

            if (roarSound != null)
                roarSound.Play();
        }

        void Update() {
            rb.angularVelocity = 0;
            if (GlobalGameData.isPaused) {
                rb.velocity = Vector2.zero;
                return;
            }

            switch (AIState) {
                case LichAIState.Follow:
                    Follow();
                    break;
                case LichAIState.Attack:
                    Attack();
                    break;
                case LichAIState.Teleport:
                    Teleport();
                    break;
                case LichAIState.Summon:
                    Summon();
                    break;
            }
        }

        public void LookAt() {
            float idealAngle = Util.AngleToPlayer(transform, player);
            rb.rotation = Mathf.MoveTowardsAngle(transform.rotation.eulerAngles.z, idealAngle, 15f);
        }

        public void Die() {
            Destroy(gameObject);
        }

        public void Move() {
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            Vector2 perpendicularDirection = new Vector2(-directionToPlayer.y, directionToPlayer.x) * circleDistance; // Perpendicular to the direction to the player

            // Move around the player in a circular motion
            rb.velocity =  speed * 1.5f * (directionToPlayer + perpendicularDirection).normalized;

            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            rb.rotation = angle + rotationOffset; // Adjust based on the enemy's default orientation
        }

        #region AI States
        public void Follow() {
            if (player != null) {
                Move();

                if (circleTimer >= maxCircleTimer) {
                    AIState = LichAIState.Attack;
                    circleTimer = 0f;
                } else {
                    circleTimer += Time.deltaTime;
                }
            }
        }

        public void Attack() {
            if (player != null) {
                if (attackTimer >= maxHomingTimer) {
                    if (counter < maxHomingCounter) {
                        AIState = LichAIState.Teleport;
                        attackTimer = 0f;
                        teleportTimer = 0f;
                        counter++;
                    } else {
                        AIState = LichAIState.Summon;
                        attackTimer = 0f;
                        counter = 0;
                    }
                } else {
                    if (attackTimer == 0) {
                        // Do attack logic here
                        float increment = 180f / homingProjectileCount;

                        for (float i = 90f; i < 270f; i += increment) {
                            GameObject projectile = Instantiate(homingProjectile, transform.position, Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 0, i)));
                            projectile.transform.parent = null;
                            projectile.GetComponent<Projectile>().InitStuff(10, 15, 15, 1, Owner.Enemy, ProjectileType.Homing, player);
                        }
                    }

                    Move();

                    attackTimer += Time.deltaTime;
                }
            }
        }
        

        public void Teleport() {
            rb.velocity = Vector2.zero;
            if (player == null) return;

            if (teleportTimer >= maxTeleportTimer) {
                AIState = LichAIState.Attack;
                teleportTimer = 0;
            } else {
                if (teleportTimer == 0) {
                    float angle = UnityEngine.Random.Range(0f, 360f);
                    Vector3 position = 12 * new Vector3(Mathf.Sin(angle), Mathf.Cos(angle));

                    transform.position = player.position + position;
                }

                LookAt();

                teleportTimer += Time.deltaTime;
            }
        }

        public void Summon() {
            rb.velocity = Vector2.zero;

            if (attackTimer >= maxSummonTimer) {
                if (counter < maxSummonCounter) {
                    attackTimer = 0;
                    counter++;
                } else {
                    AIState = LichAIState.Follow;
                    attackTimer = 0f;
                    counter = 0;
                }
            } else {
                if (attackTimer == 0) {
                    float angle = 360 / fleshLumpCount;
                    for (int i = 0; i < fleshLumpCount; i++) {
                        Vector3 position = 4 * new Vector3(Mathf.Sin(angle * i), Mathf.Cos(angle * i));
                        Instantiate(fleshLump, transform.position + position, transform.rotation, null);
                    }
                }

                LookAt();

                attackTimer += Time.deltaTime;
            }
        }
        #endregion

        void OnTriggerEnter2D(Collider2D other)
        {
            if (GlobalGameData.isPaused) return;
            
            CallDamage(other);
        }

        void OnCollisionEnter2D(Collision2D other)
        {
            if (GlobalGameData.isPaused) return;
            
            if (other.gameObject.TryGetComponent<Player>(out var player)) {
                player.DirectDamage((int)attackDamage);
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


        #region Static Lich Type Functions
        public static int LichMaxHealth(LichType type) {
            return type switch
            {
                LichType.Aserik => 12000,
                _ => 1000,
            };
        }

        public static int LichXP(LichType type) {
            return type switch
            {
                LichType.Aserik => 1000,
                _ => 150,
            };
        }
        #endregion
    }

    [System.Serializable]
    public enum LichAIState {
        Follow,
        Attack,
        Teleport,
        Summon
    }

    [System.Serializable]
    public enum LichType {
        Aserik
    }
}