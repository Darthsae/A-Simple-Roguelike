using System;
using UnityEngine;

namespace ASimpleRoguelike.Entity.Bosses {
    public class RotnotHandController : Entity {
        public Rigidbody2D rigidbodyToUse;
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

        private bool broken = false;
        public SpriteRenderer spriteWeapon;
        public Sprite brokenWeaponSprite;
        public SpriteRenderer spriteArm;
        public Sprite brokenArmSprite;
        public SpriteRenderer spriteForearm;
        public Sprite brokenForearmSprite;
        public SpriteRenderer spriteHand;
        public Sprite brokenHandSprite;

        void Start() {
            health.OnHealthZero += Die;
            rb = rigidbodyToUse;
        }

        private void Die() {
            broken = true;
            spriteWeapon.sprite = brokenWeaponSprite;
            spriteArm.sprite = brokenArmSprite;
            spriteForearm.sprite = brokenForearmSprite;
            spriteHand.sprite = brokenHandSprite;
        }

        public void Use() {
            inUse = true;
            if (animationToPlay != null) {
                animationToPlay.Play();
            }
        }

        public override void UpdateAI() {
            base.UpdateAI();

            if (!inUse) return;

            timeInUse += Time.deltaTime;
            
            if (timeInUse >= time || broken) {
                inUse = false;
                saver = 0f;
                timeInUse = 0f;
                PostUse?.Invoke();
                return;
            }

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
            
            if (!shoots) {
                return;
            }
            
            saver -= Time.deltaTime;
            
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
                health.ChangeHealth((int)-projectile.damage);

                nextInvulnerabilityTime = Time.time + invulnerabilityDuration;

                if (hurtSound != null) {
                    hurtSound.Play();
                }
            }
        }
    }
}