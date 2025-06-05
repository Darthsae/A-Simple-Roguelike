using UnityEngine;

namespace ASimpleRoguelike.Entity {
    public class Worm : MonoBehaviour {
        public Enemy leader;
        public Transform leaderTransform;
        public Collider2D bodyCollider;
        public Collider2D damageCollider;
        public SpriteRenderer sprite;
        public float distance;
        public float rotationOffest = -90f;

        public void Init() {
            if (leader != null) {
                leader.health.OnHealthZero += () => Destroy(gameObject);
                sprite = GetComponent<SpriteRenderer>();
            } else {
                Destroy(gameObject);
            }
        }
        
        void Update() {
            Vector2 directionToLeader = leaderTransform.position - transform.position;
            float distanceToLeader = directionToLeader.magnitude;
            if (distanceToLeader > distance * 0.95f) {
                directionToLeader = directionToLeader.normalized * (distanceToLeader - distance);
            } else if (distanceToLeader < distance * 0.95f) {
                directionToLeader = -directionToLeader.normalized * (distanceToLeader - distance);
            }
            transform.position += (Vector3)directionToLeader;

            Vector2 direction = (leaderTransform.position - transform.position).normalized;

            // Calculate the angle to rotate
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Apply the rotation, only affecting the z-axis
            transform.rotation = Quaternion.Euler(0f, 0f, angle + rotationOffest);
        }

        public void CallBurrow() {
            bodyCollider.enabled = false;
            sprite.enabled = false;
        }

        public void CallUnburrow() {
            bodyCollider.enabled = true;
            sprite.enabled = true;
        }

        public void OnTriggerEnter2D(Collider2D other) {
            leader.CallDamage(other);
        }
    }
}