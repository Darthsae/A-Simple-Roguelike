using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using ASimpleRoguelike.StatusEffects;

namespace ASimpleRoguelike {
    public class Projectile : MonoBehaviour {
        public int split = -1;
        public float speed = 10;
        public float damage = 1;
        public float duration = 10;
        public float homingSpeed = 45f;
        public int piercing = 1;
        private Rigidbody2D rb;
        [SerializeField]
        private float timer = 0.0f;
        //private Vector2 last;
        public Owner owner;
        public ProjectileType type = ProjectileType.Normal;
        public Transform target;
        public List<StatusEffectData> effects = new();
        
        public float minSpeed = 0.75f;
        public float currentDir = 0;
        public float dirChangeFactor = 0.45f;

        public float angularity = 0f;

        public Vector2 oldVelocity = Vector2.zero;

        public AnimationCurve accelCurve = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 1.0f);
        public AnimationCurve speedCurve = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 1.0f);
        public AnimationCurve homingSpeedCurve = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 1.0f);

        public LayerMask enemyLayer;

        public float accelSpeed = 2.5f;

        private void Start() {
            rb = GetComponent<Rigidbody2D>();
        }

        public void InitStuff(float speed = 10, float damage = 1, float duration = 10, int piercing = 1, Owner owner = Owner.Player, ProjectileType type = ProjectileType.Normal, Transform target = null) {
            this.speed = speed;
            this.damage = damage;
            this.duration = duration;
            this.piercing = piercing;
            this.owner = owner;
            this.type = type;
            this.target = target;
            timer = duration;

            if (owner == Owner.Enemy) {
                if (TryGetComponent<Collider2D>(out var collider)) {
                    // The enemy layer mask is called: "Enemy", add it to excludeLayers
                    collider.excludeLayers |= enemyLayer;
                }
            }

            //last = transform.position;
        }

        public void SetScale(float scale) {
            transform.localScale *= scale;
        }

        void Update() {
            if (GlobalGameData.isPaused) {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0;
                return;
            }

            timer -= Time.deltaTime;

            /*
            if (last != null) {
                RaycastHit2D hit = Physics2D.Linecast(last, transform.position);
                if (hit.collider != null && hit.collider.gameObject.GetComponent<Enemy>() != null) {
                    hit.collider.gameObject.GetComponent<Enemy>().CallDamage(gameObject.GetComponent<Collider2D>());
                }
            }
            */
            switch (type) {
                case ProjectileType.Normal:
                    rb.velocity = transform.right * speed;
                    break;
                case ProjectileType.Homing:
                    float idealAngle = Entity.Util.AngleToPlayer(transform, target);
                    float angular = Mathf.Abs(Mathf.DeltaAngle(rb.rotation, idealAngle)) / 180f;
                    currentDir = 1.0f - angular;
                    angularity = angular;
                    rb.rotation = Mathf.MoveTowardsAngle(rb.rotation, idealAngle, homingSpeed * homingSpeedCurve.Evaluate(1.0f - angular) * Time.deltaTime);
                    Vector2 desired = speed * speedCurve.Evaluate(currentDir) * transform.right;
                    float accel = accelSpeed * accelCurve.Evaluate(currentDir) * Time.deltaTime;
                    rb.velocity = Vector2.MoveTowards(oldVelocity, desired, accel);
                    oldVelocity = rb.velocity;
                    break;
            }

            if (timer <= 0) {
                Destroy(gameObject);
            }
            
            //last = transform.position;
        }

        public void Hit() {
            piercing--;
            if (piercing == 0) {
                Destroy(gameObject);
            }
        }

        
        void OnDrawGizmos() {
            switch (type) {
                case ProjectileType.Homing:
                    float idealAngle = Entity.Util.AngleToPlayer(transform, target);
                    Vector3 direction = (target.position - transform.position).normalized;
                    
                    float angular = Mathf.Abs(Mathf.DeltaAngle(rb.rotation, idealAngle)) / 180f;
                    angularity = angular;
                    currentDir = 1.0f - angular;
                    
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(transform.position, transform.position + direction * 0.5f);
                    Gizmos.DrawLine(transform.position + new Vector3(Mathf.Cos(idealAngle * Mathf.Deg2Rad), Mathf.Sin(idealAngle * Mathf.Deg2Rad)) * 1.0f, transform.position + new Vector3(Mathf.Cos(idealAngle * Mathf.Deg2Rad), Mathf.Sin(idealAngle * Mathf.Deg2Rad)) * 1.5f);
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(transform.position, transform.position + transform.right * 0.5f);
                    Gizmos.DrawLine(transform.position + new Vector3(Mathf.Cos(rb.rotation * Mathf.Deg2Rad), Mathf.Sin(rb.rotation * Mathf.Deg2Rad)) * 1.0f, transform.position + new Vector3(Mathf.Cos(rb.rotation * Mathf.Deg2Rad), Mathf.Sin(rb.rotation * Mathf.Deg2Rad)) * 1.5f);
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(transform.position + transform.right * 0.5f, transform.position + transform.right * (0.5f + 0.5f * speedCurve.Evaluate(currentDir)));
                    
                    Gizmos.color = Color.black;
                    float nextAngle = rb.rotation;
                    for (int i = 0; i < 60; i++) {
                        angular = Mathf.Abs(Mathf.DeltaAngle(nextAngle, idealAngle)) / 180f;
                        nextAngle = Mathf.MoveTowardsAngle(nextAngle, idealAngle, homingSpeedCurve.Evaluate(1.0f - angular) * homingSpeed * 0.015f);
                        Gizmos.DrawLine(transform.position + new Vector3(Mathf.Cos(nextAngle * Mathf.Deg2Rad), Mathf.Sin(nextAngle * Mathf.Deg2Rad)) * 0.5f, transform.position + new Vector3(Mathf.Cos(nextAngle * Mathf.Deg2Rad), Mathf.Sin(nextAngle * Mathf.Deg2Rad)));
                    }
                    break;
            }
        }
    }

    [System.Serializable]
    public enum Owner {
        Player,
        Enemy
    }

    [System.Serializable]
    public enum ProjectileType {
        Normal,
        Homing,
        Turning
    }
}