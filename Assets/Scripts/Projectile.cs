using System.Collections;
using UnityEngine;
using ASimpleRoguelike.Entity;

namespace ASimpleRoguelike {
    public class Projectile : MonoBehaviour
    {
        public float speed = 10;
        public float damage = 1;
        public float duration = 10;
        public float homingSpeed = 45f;
        public int piercing = 1;
        private Rigidbody2D rb;
        private Vector2 last;
        public Owner owner;
        public ProjectileType type = ProjectileType.Normal;
        public Transform target;

        private void Start()
        {
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
            StartCoroutine(DestroyAfterTime());

            last = transform.position;
        }

        public void SetScale(float scale) {
            transform.localScale *= scale;
        }

        IEnumerator DestroyAfterTime() {
            yield return new WaitForSeconds(duration);
            Destroy(gameObject);
        }

        void Update()
        {
            if (GlobalGameData.isPaused) {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0;
                return;
            }
            
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
                    float idealAngle = Util.AngleToPlayer(transform, target);
                    rb.rotation = Mathf.MoveTowardsAngle(transform.rotation.eulerAngles.z, idealAngle, homingSpeed * Time.deltaTime);
                    rb.velocity = transform.right * speed;
                    break;
                
            }
            
            last = transform.position;
        }

        public void Hit() {
            piercing--;
            if (piercing == 0) {
                Destroy(gameObject);
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