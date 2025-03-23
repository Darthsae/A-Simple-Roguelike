using System;
using System.Collections;
using UnityEngine;

namespace ASimpleRoguelike.Entity.Bosses {
    public class RotnotHandController : MonoBehaviour {
        public RotnotController rotnotController;
        public Animation animationToPlay;
        public RotnotHand type;
        public float time;
        public float timeInUse;
        public float saver = 0.0f;
        public float fireTime = 0.5f;
        public GameObject projectile;
        public Transform firePoint;
        public bool canMoveDuring = true;
        public bool shoots = false;
        public event Action PostUse;

        public float projectileSpeed = 10f;
        public float projectileDamage = 10;
        public float projectileDuration = 3f;
        public int projectilePiercing = 1;
        public float projectileScale = 1f;

        public Transform temp;

        private bool inUse = false;

        public void Use() {
            inUse = true;
            if (animationToPlay != null) animationToPlay.Play();
            StartCoroutine(UseCoroutine());
        }

        void Update() {
            if (GlobalGameData.isPaused) return;

            if (!inUse) return;

            timeInUse += Time.deltaTime;

            switch (type) {
                case RotnotHand.Clipper:
                    rotnotController.RightHandIK(rotnotController.player.position);
                    break;
                case RotnotHand.Cannon:
                    rotnotController.RightHandIK(rotnotController.player.position);
                    break;
                case RotnotHand.Gatling:
                    rotnotController.LeftHandIK(2 * ((timeInUse / (time * 2)) - 0.5f));
                    break;
                case RotnotHand.Sword:
                    //rotnotController.LeftHandIK(rotnotController.player.position);
                    break;
            }
            
            if (shoots) {
                if (saver < 0f) {
                    saver = fireTime;

                    Vector2 direction = (Vector2)firePoint.transform.position - (Vector2)transform.position;
                    direction.Normalize();

                    // Calculate the angle for each projectile with spread
                    float directionAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                    // Instantiate the projectile
                    GameObject clone = Instantiate(projectile, firePoint.transform.position, Quaternion.identity);
                    clone.transform.parent = null;

                    // Set position and rotation
                    clone.transform.SetPositionAndRotation(transform.position, Quaternion.Euler(new Vector3(0, 0, directionAngle)));

                    // Set the projectile velocity
                    clone.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;

                    // Initialize projectile properties
                    clone.GetComponent<Projectile>().InitStuff(projectileSpeed, projectileDamage, projectileDuration, projectilePiercing, Owner.Enemy);
                    clone.GetComponent<Projectile>().SetScale(projectileScale);
                }
                else {
                    saver -= Time.deltaTime;
                }
            }
        }

        private IEnumerator UseCoroutine() {
            yield return new WaitForSeconds(time);
            inUse = false;
            saver = 0f;
            timeInUse = 0f;
            PostUse?.Invoke();
        }
    }
}