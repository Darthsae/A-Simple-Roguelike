using UnityEngine;

namespace ASimpleRoguelike.Entity {
    public class PlayerMinion : Entity {
        public MinionAIType AIType = MinionAIType.Follow;
        public float rotationOffset = -90f;
        public float speed = 10f;
        public float attackRange = 2f;
        public float attackDamage = 10f;
        public float attackRate = 1f;
        public float targetRange = 10f;
        public float maxDistanceFromPlayer = 15f;
        private Transform player;
        private float nextAttackTime = 0f;
        private Transform target;
        

        private void Start() {
            rb = GetComponent<Rigidbody2D>();
            player = GameObject.FindGameObjectWithTag("Player").transform;
            GetTarget();
            health.OnHealthZero += Die;
        }

        public void GetTarget() {
            foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, targetRange)) {
                if (collider.gameObject.TryGetComponent<Enemy>(out var enemy) && Vector2.Distance(player.position, enemy.transform.position) <= maxDistanceFromPlayer) {
                    target = enemy.transform;
                    break;
                }
            }
        }

        public override void UpdateAI() {
            switch (AIType) {
                case MinionAIType.Follow:
                    MoveTowardsTarget();
                    if (IsTargetInRange() && Time.time >= nextAttackTime) {
                        AttackTarget();
                        nextAttackTime = Time.time + 1f / attackRate;
                    }
                    break;
            }
        }

        void OnCollisionEnter2D(Collision2D other) {
            if (other.gameObject.TryGetComponent<Enemy>(out var enemy)) {
                DirectDamage(-(int)enemy.attackDamage);
            }
        }

        private void MoveTowardsTarget() {
            if (target != null) {
                Vector2 direction = (target.position - transform.position).normalized;
                rb.velocity = direction * speed;

                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                rb.rotation = angle + rotationOffset; // Adjust based on the enemy's default orientation
            } else {
                GetTarget();
            }
        }

        private bool IsTargetInRange() {
            if (target != null) {
                float distance = Vector2.Distance(transform.position, target.position);
                return distance <= attackRange;
            }
            return false;
        }

        private void AttackTarget() {
            // Assuming the player has a Health component
            if (target.TryGetComponent<Health>(out var playerHealth)) {
                //Debug.Log("Enemy health: " + playerHealth.health);
                playerHealth.ChangeHealth((int)-attackDamage);
            }
        }

        private void Die() {
            Destroy(gameObject);
        }
    }

    [System.Serializable]
    public enum MinionAIType {
        Follow,
        Circle,
        Harrass,
        Charge
    }
}