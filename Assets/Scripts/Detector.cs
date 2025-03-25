using ASimpleRoguelike.Entity;
using UnityEngine;

namespace ASimpleRoguelike {
    [RequireComponent(typeof(Enemy))]
    public class Detector : MonoBehaviour {
        public string identifier = "";
        public GlobalGameData globalGameData;
        public Enemy entity;

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

        public bool hasLaser = true;
        public bool chargingLaser = false;
        public bool canMoveWhileChargingLaser = false;
        public bool canMoveWhileUsingLaser = false;
        public bool usingLaser = false;
        public bool laserOnCooldown = false;

        void Awake() {
            entity = GetComponent<Enemy>();
            globalGameData = FindFirstObjectByType<GlobalGameData>();
        }

        public void Update() {
            if (GlobalGameData.isPaused) {
                return;
            }

            UpdateLaser();
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

                    entity.RemoveMovementBlocker("ChargingLaser" + identifier);
                    if (!canMoveWhileUsingLaser) {
                        entity.AddMovementBlocker("UsingLaser" + identifier);
                    }
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
                    
                    entity.RemoveMovementBlocker("UsingLaser" + identifier);
                }
            } else if (laserDetector.GetComponent<BoxCollider2D>().IsTouching(globalGameData.player.GetComponent<Collider2D>())) {
                chargingLaser = true;
                laserTimer = laserCharge;
                laserChargeSound.Play();
                laserParticles.Play();
            
                if (!canMoveWhileChargingLaser) {
                    entity.AddMovementBlocker("ChargingLaser" + identifier);
                }
            }
        }
    }
}