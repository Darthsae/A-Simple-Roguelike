using System;
using UnityEngine;

namespace ASimpleRoguelike {
    [RequireComponent(typeof(CircleCollider2D))]
    public class RingCollider : MonoBehaviour
    {
        public event Action<Collider2D> onCollide;
        public float innerRadius;

        private void OnTriggerStay2D(Collider2D collision)
        {
            float distance = Vector2.Distance(collision.transform.position, transform.position); 

            if (distance >= innerRadius * gameObject.transform.localScale.x)
            {
                onCollide?.Invoke(collision);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, innerRadius * gameObject.transform.localScale.x);
        }
    }
}