using System;
using Unity.Mathematics;
using UnityEngine;

namespace ASimpleRoguelike.Movement {
    public class FacingMovementController : MovementController {
        public float minSpeed = 0.75f;

        public float currentDir = 0;

        public float dirChangeFactor = 0.05f;

        public void Awake() {
            displayName = "Facing";
        }
        
        public override void HandleMovement(Rigidbody2D rigidbody) {
            float moveX = Input.GetAxis("Horizontal");
            float moveY = Input.GetAxis("Vertical");

            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (Vector2)mousePosition - (Vector2)player.transform.position;
            direction.Normalize();
            Vector2 sideDirection = Vector2.Perpendicular(direction);

            Vector2 movement = new(moveX, moveY);
            movement.Normalize();
            Vector2 finalMove = -movement.x * sideDirection + movement.y * direction;
            currentDir = Mathf.Lerp(currentDir, (float)(minSpeed + (1.0 - minSpeed) * ((finalMove.normalized + direction.normalized).magnitude / 2)), dirChangeFactor);
            rigidbody.velocity = (speed.value + tempSpeed) * currentDir * finalMove;

            float tempPitch = movement.magnitude;

            if (movement != Vector2.zero) {
                if (GlobalGameData.neckSlot != null && GlobalGameData.neckSlot.name == "DiscordantPendant") {
                    if (Input.GetKeyDown(KeyCode.LeftControl)) {
                        isRushing = true;
                        rushIndicator.SetActive(true);
                        rushMover.SetActive(true);
                        rushMover.transform.localPosition = Vector3.zero;
                    } else if (Input.GetKeyUp(KeyCode.LeftControl)) {
                        isRushing = false;
                        player.transform.position = rushMover.transform.position;
                        rushIndicator.SetActive(false);
                        rushMover.SetActive(false);
                    }
                } else {
                    if (Input.GetKey(KeyCode.LeftShift) && player.stamina.stamina > 0 && !player.pauseStaminaRegen) {
                        rigidbody.velocity *= 1.5f; 

                        tempPitch *= 1.5f;

                        if (player.stamTimer > 0) {
                            player.stamTimer -= Time.deltaTime;
                        } else {
                            player.stamina.ChangeStamina(-1);
                            player.stamTimer = player.stamDelay;
                        }
                    } else if (player.pauseStaminaRegen) {
                        rigidbody.velocity *= 0.9f;
                    }
                }
                
                moveSound.pitch = tempPitch; 
                if (!moveSound.isPlaying) { 
                    moveSound.Play();
                }
            }

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            player.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            if (isRushing) {
                rushIndicator.transform.localPosition = rigidbody.velocity * russianSpeed;
            }
        }
    }
}