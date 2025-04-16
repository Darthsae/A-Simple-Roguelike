using UnityEngine;

namespace ASimpleRoguelike.Entity.Bosses {
    public class RotnotController : Boss {
        #region Locamotion Info
        public float speed = 10f;
        public float turningSpeed = 180f;

        public float maxArmAngleDelta = 40f;
        public float maxForearmAngleDelta = 130f;

        public bool brainDead = false;
        #endregion

        #region Attack Info
        public float attackDelayTime = 0.5f;
        private float nextAttackTime = 0f;
        private bool leftHandAttack = true;
        public bool shouldMove = true;
        public float knockback = 10f;

        public float bombDelayTime = 1.5f;
        public float bombTimer = 0f;

        public int maxSummon = 4;
        public int summonCounter = 0;

        public GameObject summondPrefab;

        public GameObject bombPrefab;
        #endregion

        #region Template Info 
        public GameObject leftSword;
        public GameObject rightCannon;
        public GameObject leftGatling;
        public GameObject rightClipper;
        #endregion

        #region Body Info
        public GameObject headObject;
        public GameObject leftArmObject;
        public GameObject rightArmObject;
        public GameObject leftForearmObject;
        public GameObject rightForearmObject;
        public GameObject leftHandObject;
        public GameObject rightHandObject;
        public float armLength = 2.25f;
        public float forearmLength = 3f;
        #endregion

        #region Sprite Info
        public SpriteRenderer bodySprite;
        public SpriteRenderer headSprite;
        public SpriteRenderer leftArmSprite;
        public SpriteRenderer rightArmSprite;
        public SpriteRenderer leftForearmSprite;
        public SpriteRenderer rightForearmSprite;
        public SpriteRenderer leftHandSprite;
        public SpriteRenderer rightHandSprite;
        #endregion

        #region Rotnot Data
        public RotnotAIState AIState;
        public RotnotType type;
        public RotnotHandController leftHand;
        public RotnotHandController rightHand;
        #endregion

        void Start() {
            rb = GetComponent<Rigidbody2D>();
            player = GameObject.FindGameObjectWithTag("Player").transform; // Assuming the player has the "Player" tag
            
            AIState = RotnotAIState.Follow;

            type = RotnotType.Ferric;

            RotnotHand leftHandType = RotnotHand.Gatling;
            RotnotHand rightHandType = RotnotHand.Cannon;

            health.SetMaxHealth(RotnotMaxHealth(type));
            health.OnHealthZero += Die;

            switch (leftHandType) {
                case RotnotHand.Gatling:
                    leftGatling.SetActive(true);
                    leftHand = leftGatling.GetComponent<RotnotHandController>();
                    break;
                case RotnotHand.Sword:
                    leftSword.SetActive(true);
                    leftHand = leftSword.GetComponent<RotnotHandController>();
                    break;
            }

            switch (rightHandType) {
                case RotnotHand.Cannon:
                    rightCannon.SetActive(true);
                    rightHand = rightCannon.GetComponent<RotnotHandController>();
                    break;
                case RotnotHand.Clipper:
                    rightClipper.SetActive(true);
                    rightHand = rightClipper.GetComponent<RotnotHandController>();
                    break;
            }

            leftHand.PostUse += () => {
                nextAttackTime = attackDelayTime;
                shouldMove = true;
                AIState = RotnotAIState.Follow;
            };

            rightHand.PostUse += () => {
                nextAttackTime = attackDelayTime;
                shouldMove = true;
                AIState = RotnotAIState.Follow;
            };
        
            xp = RotnotXP(type);
        }

        public override void UpdateAI() {
            base.UpdateAI();
            rb.angularVelocity = 0;

            if (brainDead) {
                //LeftHandIK(player.position);
                RightHandIK(player.position);
                return;
            }

            if (health.health <= health.maxHealth * 0.5f) {
                if (bombTimer >= bombDelayTime) {
                    bombTimer = 0f;
                    for (int i = 0; i < 10; i++) {
                        float angle = Random.Range(0, 360);
                        GameObject bomb = Instantiate(bombPrefab, transform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * 4.5f, Quaternion.identity);
                        bomb.transform.Rotate(new(0, 0, angle));
                        bomb.GetComponent<Projectile>().InitStuff(6f, 35f, 2, 1, Owner.Enemy, ProjectileType.Normal);
                    }

                    if (summonCounter >= maxSummon) {
                        summonCounter = 0;
                        for (int i = 0; i < 8; i++) {
                            GameObject summon = Instantiate(summondPrefab, transform.position + (Vector3)Random.insideUnitCircle * 12.5f, Quaternion.identity);
                            summon.GetComponent<Enemy>().speed *= 1.5f;
                            summon.GetComponent<Enemy>().health.SetMaxHealth(5);
                            summon.GetComponent<Enemy>().AIType = EnemyAIType.ZigFollow;
                        }
                    } else {
                        summonCounter++;
                    }
                } else {
                    bombTimer += Time.deltaTime;
                }
            }

            switch (AIState) {
                case RotnotAIState.Follow:
                    Follow();
                    break;
                case RotnotAIState.Attack:
                    Attack();
                    break;
            }
        }

        public void Die() {
            Destroy(gameObject);
        }

        #region AI States
        public void Follow() {
            if (player == null) return;

            rb.velocity = -transform.up * speed;

            float idealAngle = Util.AngleToPlayer(transform, player);

            headObject.transform.rotation = Quaternion.Euler(0, 0, idealAngle);
            rb.rotation = Mathf.MoveTowardsAngle(transform.rotation.eulerAngles.z, idealAngle, turningSpeed * Time.deltaTime);

            if (nextAttackTime > 0) {
                nextAttackTime -= Time.deltaTime;
            }
            else {
                AIState = RotnotAIState.Attack;
                switch (leftHandAttack) {
                    case true:
                        leftHand.Use();
                        leftHandAttack = false;
                        shouldMove = leftHand.canMoveDuring;
                        break;
                    case false:
                        rightHand.Use();
                        leftHandAttack = true;
                        shouldMove = rightHand.canMoveDuring;
                        break;
                }
            }
        }

        public void Attack() {
            if (player == null) return;

            if (shouldMove) {
                float idealAngle = Util.AngleToPlayer(transform, player);

                rb.rotation = Mathf.MoveTowardsAngle(transform.rotation.eulerAngles.z, idealAngle, turningSpeed * Time.deltaTime);
                headObject.transform.rotation = Quaternion.Euler(0, 0, idealAngle);
            }
        }
        #endregion

        #region IK
        public void LeftHandIK(float times) {
            float min = 35f;
            float max = 65f;

            float angleArm = Mathf.Lerp(-min, min, 0.5f + times * 0.5f);
            float forearmAngle = Mathf.Lerp(-max, max, 0.5f + times * 0.5f);

            //print("Time: " + times);

            leftArmObject.transform.localRotation = Quaternion.Euler(0, 0, 45 + angleArm);
            leftForearmObject.transform.localRotation = Quaternion.Euler(0, 0, forearmAngle);
        }

        public void LeftHanderIK(Vector2 globalTargetPosition) {
            float minArmAngle = 90 - maxArmAngleDelta;
            float maxArmAngle = 105f;

            Vector2 globalArmPosition = leftArmObject.transform.position;

            float globalPreviousAngleOfArmToTarget = Util.ActualAngle(globalArmPosition, globalTargetPosition);

            //float globalAngleOfArmToTarget = Mathf.Clamp(globalPreviousAngleOfArmToTarget, minArmAngle, maxArmAngle);

            float armAngle = Mathf.Clamp(globalPreviousAngleOfArmToTarget, minArmAngle, maxArmAngle);

            //Debug.Log("LAngle0: " + globalPreviousAngleOfArmToTarget);
            //Debug.Log("LAngle1: " + localRotation.eulerAngles.z);
            //Debug.Log("LAngle2: " + armAngle);

            // Set global rotation of arm
            leftArmObject.transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + armAngle - 90f);

            float minForearmAngle = -maxForearmAngleDelta;
            float maxForearmAngle = 0;

            Vector2 globalForearmPosition = leftForearmObject.transform.position;

            float globalPreviousAngleOfForearmToTarget = Util.ActualAngle(globalForearmPosition, globalTargetPosition);
            leftForearmObject.transform.rotation = Quaternion.Euler(0, 0, globalPreviousAngleOfForearmToTarget);
            float usableForearmAngle = Mathf.Clamp(leftForearmObject.transform.localRotation.eulerAngles.z, minForearmAngle, maxForearmAngle);

            //Debug.Log("LAngle0: " + globalPreviousAngleOfForearmToTarget);
            //Debug.Log("LAngle1: " + localForearmRotation.eulerAngles.z);
            //Debug.Log("LAngle2: " + forearmAngle);

            leftForearmObject.transform.localRotation = Quaternion.Euler(0, 0, usableForearmAngle);
        }

        public void LeftHandOtherIK(Vector2 globalTargetPosition) {
            float minArmAngle = -maxArmAngleDelta;
            float maxArmAngle = 15f;

            Vector2 globalArmPosition = leftArmObject.transform.position;

            float globalPreviousAngleOfArmToTarget = Util.ActualAngle(globalArmPosition, globalTargetPosition);
            float globalAngleOfArmToTarget = Mathf.Clamp(globalPreviousAngleOfArmToTarget, minArmAngle, maxArmAngle);

            //Debug.Log("LAngle0: " + globalPreviousAngleOfArmToTarget);

            // Set global rotation of arm
            leftArmObject.transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + 90f + globalAngleOfArmToTarget);

            if (globalPreviousAngleOfArmToTarget == globalAngleOfArmToTarget) {
                leftForearmObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
                return;
            }

            float minForearmAngle = 360f - maxForearmAngleDelta;
            float maxForearmAngle = 360f;

            Vector2 globalForearmPosition = leftForearmObject.transform.position;

            float globalPreviousAngleOfForearmToTarget = Util.ActualAngle(globalForearmPosition, globalTargetPosition) + 90f;

            leftForearmObject.transform.rotation = Quaternion.Euler(0, 0, globalPreviousAngleOfForearmToTarget);
            float usableForearmAngle = Mathf.Clamp(leftForearmObject.transform.localRotation.eulerAngles.z, minForearmAngle, maxForearmAngle);

            //Debug.Log("LAngle1: " + globalPreviousAngleOfForearmToTarget);
            //Debug.Log("LAngle2: " + leftForearmObject.transform.localRotation.eulerAngles.z);
            //Debug.Log("LAngle3: " + usableForearmAngle);

            leftForearmObject.transform.localRotation = Quaternion.Euler(0, 0, usableForearmAngle);
        }

        public void LeftHandOldIK(Vector2 target) {
            float minArmAngle = 90 + maxArmAngleDelta;
            float maxArmAngle = 90 - 15f;
        
            float minForearmAngle = 0;
            float maxForearmAngle = maxForearmAngleDelta;

            // Step 1: Calculate the distance between the arm base and the target
            Vector2 armBasePosition = leftArmObject.transform.position;
            float targetDistance = Vector2.Distance(armBasePosition, target);



            // Step 2: Clamp the target distance to be within the reach of the arm and forearm
            targetDistance = Mathf.Clamp(targetDistance, Mathf.Abs(armLength - forearmLength), armLength + forearmLength);

            // Step 3: Law of Cosines to find the angles
            float cosAngleArm = (Mathf.Pow(targetDistance, 2) + Mathf.Pow(armLength, 2) - Mathf.Pow(forearmLength, 2)) / (2 * targetDistance * armLength);
            float cosAngleForearm = (Mathf.Pow(armLength, 2) + Mathf.Pow(forearmLength, 2) - Mathf.Pow(targetDistance, 2)) / (2 * armLength * forearmLength);

            float angleArm = Mathf.Acos(cosAngleArm) * Mathf.Rad2Deg;          // The internal angle at the arm's base
            float angleForearm = Mathf.Acos(cosAngleForearm) * Mathf.Rad2Deg;  // The internal angle at the forearm's base

            // Step 4: Calculate the direction from the arm base to the target
            float shoulderToTargetAngle = Vector2.Angle(armBasePosition, target);

            //Debug.Log(shoulderToTargetAngle);

            // Step 5: Apply angle constraints for the arm
            float finalArmAngle = shoulderToTargetAngle - angleArm;  // This is the actual angle for the arm based on the target

            // Clamp the final arm angle between the constraints (130° to 50°)
            finalArmAngle = Mathf.Clamp(finalArmAngle, minArmAngle, maxArmAngle);

            // Step 6: Calculate the forearm's rotation relative to the arm
            float finalForearmAngle = angleForearm - 90f;  // Since the forearm rotates relative to the arm's orientation

            // Clamp the final forearm angle between the constraints (-100° to 100°)
            finalForearmAngle = Mathf.Clamp(finalForearmAngle, minForearmAngle, maxForearmAngle);

            //Debug.Log(finalArmAngle);

            //Debug.Log((finalArmAngle + finalForearmAngle));

            // Step 7: Apply the rotations
            leftArmObject.transform.rotation = Quaternion.Euler(0, 0, finalArmAngle);   // Rotate the arm to the clamped angle
            leftForearmObject.transform.rotation = Quaternion.Euler(0, 0, finalArmAngle + finalForearmAngle); // Rotate the forearm relative to the arm
        }

        public void LeftHandRedoneIK(Vector2 target) {
            float minArmAngle = 90 - maxArmAngleDelta;
            float maxArmAngle = 90 + 15f;
        
            float minForearmAngle = -maxForearmAngleDelta;
            float maxForearmAngle = 0;

            // Can't make new transforms, and don't know how to get a Rotate 
            Vector3 targetPositionUnrotated = player.position;
            Vector3 armPosition = leftArmObject.transform.position;

            // Get all units lined up
            armPosition -= transform.position;
            targetPositionUnrotated -= transform.position + armPosition;

            // Do the rotations
            Vector3 targetPosition = Util.RotateAroundOrigin(armPosition, targetPositionUnrotated, -gameObject.transform.rotation.eulerAngles.z, Vector2.up);

            if (targetPosition.y < 0) { // In front of the Rotnot
                if (Vector3.Distance(armPosition, targetPosition) > armLength + forearmLength) {
                    float angleTo = Vector3.Angle(armPosition, targetPosition);
                    float clampedAngleTo = Mathf.Clamp(angleTo, minArmAngle, maxArmAngle);
                    leftArmObject.transform.rotation = Quaternion.Euler(0, 0, clampedAngleTo);

                    if (angleTo > clampedAngleTo || angleTo < clampedAngleTo) {
                        float angle = Vector3.Angle(leftForearmObject.transform.position + forearmLength * new Vector3(Mathf.Cos(clampedAngleTo), Mathf.Sin(clampedAngleTo), 0), targetPosition); // Should be the ;
                        //Debug.Log(angle);
                        leftForearmObject.transform.rotation = Quaternion.Euler(0, 0, clampedAngleTo + Mathf.Clamp(angle, minForearmAngle, maxForearmAngle));
                    }
                }
                else { // Placeholder
                    leftArmObject.transform.rotation = Quaternion.Euler(0, 0, 90);
                    leftForearmObject.transform.rotation = Quaternion.Euler(0, 0, 90);
                }
            }
            else if (targetPosition.y > 0) { // Behind the Rotnot
                leftArmObject.transform.rotation = Quaternion.Euler(0, 0, 90);
                leftForearmObject.transform.rotation = Quaternion.Euler(0, 0, 90);
            }
        }

        public void RightHandIK(Vector2 globalTargetPosition) {
            float minArmAngle = -15f;
            float maxArmAngle = maxArmAngleDelta;

            Vector2 globalArmPosition = rightArmObject.transform.position;

            float globalPreviousAngleOfArmToTarget = Util.ActualAngle(globalTargetPosition, globalArmPosition);
            float globalAngleOfArmToTarget = Mathf.Clamp(globalPreviousAngleOfArmToTarget, minArmAngle, maxArmAngle);

            // Set global rotation of arm
            rightArmObject.transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + globalAngleOfArmToTarget - 90f);

            //Debug.Log("RAngle0: " + globalPreviousAngleOfArmToTarget);
            //Debug.Log("Angle2: " + globalAngleOfArmToTarget);
            //Debug.Log("Angle3: " + (globalAngleOfArmToTarget - 90f));

            float minForearmAngle = 0f;
            float maxForearmAngle = 130f;

            Vector2 globalForearmPosition = rightForearmObject.transform.position;

            float globalPreviousAngleOfForearmToTarget = Util.ActualAngle(globalTargetPosition, globalForearmPosition) - 90f;
            rightForearmObject.transform.rotation = Quaternion.Euler(0, 0, globalPreviousAngleOfForearmToTarget);
            float usableForearmAngle = Mathf.Clamp(rightForearmObject.transform.localRotation.eulerAngles.z, minForearmAngle, maxForearmAngle);

            //Debug.Log("RAngle1: " + globalPreviousAngleOfForearmToTarget);
            //Debug.Log("RAngle2: " + rightForearmObject.transform.localRotation.eulerAngles.z);
            //Debug.Log("RAngle3: " + usableForearmAngle);

            rightForearmObject.transform.localRotation = Quaternion.Euler(0, 0, usableForearmAngle);
        }

        public void RightHandOldIK(Vector2 target) {
            // Step 1: Calculate the distance between the shoulder and the target
            Vector2 shoulderPosition = rightArmObject.transform.position;
            float targetDistance = Vector2.Distance(shoulderPosition, target);

            // Step 2: Clamp the target distance to be within the reach of the arm and forearm
            targetDistance = Mathf.Clamp(targetDistance, Mathf.Abs(armLength - forearmLength), armLength + forearmLength);

            // Step 3: Law of Cosines to find the angles
            float cosAngleArm = (Mathf.Pow(targetDistance, 2) + Mathf.Pow(armLength, 2) - Mathf.Pow(forearmLength, 2)) / (2 * targetDistance * armLength);
            float cosAngleForearm = (Mathf.Pow(armLength, 2) + Mathf.Pow(forearmLength, 2) - Mathf.Pow(targetDistance, 2)) / (2 * armLength * forearmLength);

            float angleArm = Mathf.Acos(cosAngleArm) * -Mathf.Rad2Deg;
            float angleForearm = Mathf.Acos(cosAngleForearm) * -Mathf.Rad2Deg;

            // Step 4: Calculate the angle from shoulder to target direction
            Vector2 shoulderToTargetDir = (target - shoulderPosition).normalized;
            float shoulderToTargetAngle = Mathf.Atan2(shoulderToTargetDir.y, shoulderToTargetDir.x) * -Mathf.Rad2Deg;

            // Step 5: Apply angle constraints
            angleArm = Mathf.Clamp(angleArm, -maxArmAngleDelta, maxArmAngleDelta);
            angleForearm = Mathf.Clamp(angleForearm, -maxForearmAngleDelta, maxForearmAngleDelta);

            // Step 6: Rotate the arm and forearm
            rightArmObject.transform.rotation = Quaternion.Euler(0, 0, shoulderToTargetAngle - angleArm);  // Rotate the arm
            rightForearmObject.transform.rotation = Quaternion.Euler(0, 0, rightArmObject.transform.eulerAngles.z + angleForearm); // Rotate the forearm relative to the arm
        }
        #endregion

        void OnTriggerEnter2D(Collider2D other) {
            if (GlobalGameData.isPaused) return;
            
            CallDamage(other);
        }

        void OnCollisionEnter2D(Collision2D other) {
            other.rigidbody.GetComponent<Rigidbody2D>().AddForce((transform.position - player.position).normalized * knockback, ForceMode2D.Impulse);
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

        #region Static Rotnot Type Functions
        public static int RotnotMaxHealth(RotnotType type) {
            return type switch
            {
                RotnotType.Ferric => 3000,
                _ => 1000,
            };
        }

        public static int RotnotXP(RotnotType type) {
            return type switch
            {
                RotnotType.Ferric => 250,
                _ => 150,
            };
        }
        #endregion
    }

    [System.Serializable]
    public enum RotnotAIState {
        Follow,
        Attack,
        Die
    }

    [System.Serializable]
    public enum RotnotHand {
        Gatling,
        Cannon,
        Clipper,
        Sword
    }

    [System.Serializable]
    public enum RotnotType {
        Argentic, // Silver
        Auric, // Gold
        Cupric, // Copper
        Ferric, // Iron
        Hydrargyric, // Mercury
        Kalic, // Potassium
        Natric, // Sodium
        Plumbic, // Lead
        Stibic, // Antimony
        Stannic, // Tin
        Wolfric, // Tungsten
        Palladic // Palladium
    }
}