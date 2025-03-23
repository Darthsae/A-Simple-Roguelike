using System;
using System.Collections;
using TMPro;
using UnityEngine;
using ASimpleRoguelike.Equinox;
using System.Linq;
using ASimpleRoguelike.Movement;
using ASimpleRoguelike.StatusEffects;

namespace ASimpleRoguelike {
    public class Player : Entity.Entity
    {
        #region Pause Reasons
        [Header("Pause Reasons")]
        public string levelUpMenu = "Level Up Menu";
        public string settingsMenu = "Settings Menu";
        public string commandsMenu = "Commands Menu";
        #endregion

        public SpriteRenderer discordMarker;

        public void SetMove(bool move) {
            GlobalGameData.moveMode = move;
        }

        #region Movement
        public AudioSource moveSound;

        #region Rush
        public GameObject rushIndicator;
        public GameObject rushMover;

        public void RushSpeed(int amount) {movementController.RushSpeed(amount); alternateMovementController.RushSpeed(amount);}
        #endregion
        public void Speed(float amount) {movementController.Speed(amount); alternateMovementController.Speed(amount);}

        public void SetSpeed(float speed) {movementController.speed.Set(speed); alternateMovementController.speed.Set(speed);}
        public void SetRushSpeed(int rushSpeed) {movementController.rushSpeed.Set(rushSpeed); alternateMovementController.rushSpeed.Set(rushSpeed);}
        public void SetRushSpeed(float rushSpeed) {SetRushSpeed((int)rushSpeed);}
        #endregion

        public MovementController movementController;
        public MovementController alternateMovementController;

        public static Vector2 vec = new(1f, 1f);
        public GameObject levelUp;
        public RectTransform xpMask;
        public RectTransform healthMask;
        public PerkManager perkManager;

        public GameObject redCard;

        public SpriteRenderer sprite;

        public GameObject commands;

        public WeaponType currentWeapon = WeaponType.MagicOne; 

        #region Weapons
        public GameObject swordObject;
        public GameObject spearObject;
        public GameObject gunMouseObject;
        public GameObject magicOneMouseObject;
        public GameObject swordMouseObject;
        public GameObject spearMouseObject;
        #endregion

        #region Indicator Objects
        public GameObject equinoxIndicator;
        #endregion

        #region Textures
        public Sprite idleSprite;
        public Sprite attackSprite;
        #endregion
        public GameObject notificationUI;

        public RectTransform staminaMask;
        public Stamina stamina;
        public GameObject projectile;
        public string deathScene = "MainMenuScene";

        public EquinoxHandler equinoxHandeler;
        public event Action UpdateHook;

        public GameObject generalUI;
        public GameObject settingsUI;

        public Play switcher;

        public TMP_Text levelText;

        #region Audio
        public AudioSource shootSound;
        public AudioSource swingSound;
        public AudioSource magicSound;
        
        public AudioSource levelUpSound;
        public AudioSource staminaExhaustedSound;

        public AudioSource lamentSound;
        #endregion

        public event Action OnDie;

        public float delayTimer = 1f;
        public float stamTimer = 0.0f;
        public float stamDelay = 0.5f;
        

        #region Equinox
        private float equinoxTimer = 0.0f;
        public float equinoxDelay = 0.5f;
        public Vector2 equinoxVector;
        #endregion


        public static int maxBarHealth = 400;
        public static int maxBarStamina = 400;
        public static int maxBarXP = 1720;

        public int level = 1;
        public int maxXP = 100;
        public int currentXP = 0;

        public int spendXP = 0;

        public bool pauseStaminaRegen = false;

        public TMP_Text spendXPText;

        [Header("Stats")]
        public float fireRateDefault = 1f;
        public float fireRateUsed;
        public IntStat fireRate;
        public IntStat staminaRegenDelay;

        public IntStat projectileDamage;
        public FloatStat projectileSpeed;
        public FloatStat projectileDuration;
        public FloatStat projectileSpread;
        public IntStat projectilePiercing;
        public IntStat projectileCount;

        public void CalculateFireRate() {
            fireRateUsed = fireRateDefault - (0.01f * fireRate.value);
        }


        public void XPIncrement(int amount) {
            spendXP += amount;
            spendXPText.text = spendXP.ToString();
        }

        public void ProjectileDamage(int amount) {if (spendXP > 0) { projectileDamage.Change(amount); swordObject.GetComponent<HarmingArea>().damage += amount; XPIncrement(-1);}}
        public void ProjectileSpeed(float amount) {if (spendXP > 0) { projectileSpeed.Change(amount); XPIncrement(-1); }}
        public void ProjectileDuration(float amount) {if (spendXP > 0) { projectileDuration.Change(amount); XPIncrement(-1); }}
        public void ProjectileSpread(float amount) {if (spendXP > 0) { projectileSpread.Change(amount); XPIncrement(-1); }}
        public void ProjectilePiercing(int amount) {if (spendXP > 0) { projectilePiercing.Change(amount); XPIncrement(-1); }}
        public void ProjectileCount(int amount) {if (spendXP > 0) { projectileCount.Change(amount); XPIncrement(-1); }}
        public void FireRate(int amount) {if (spendXP > 0) { fireRate.Change(amount); CalculateFireRate(); XPIncrement(-1); }}
        public void StaminaRegenDelay(int amount) {if (spendXP > 0) { staminaRegenDelay.Change(amount); XPIncrement(-1); }}

        public void SetMovementController(MovementController movementController, MovementController alternateMovementController) {
            this.movementController = movementController;

            this.movementController.moveSound = moveSound;
            this.movementController.rushMover = rushMover;
            this.movementController.rushIndicator = rushIndicator;
            this.movementController.PreStart();
            
            this.movementController.PostStart();

            this.alternateMovementController = alternateMovementController;

            this.alternateMovementController.moveSound = moveSound;
            this.alternateMovementController.rushMover = rushMover;
            this.alternateMovementController.rushIndicator = rushIndicator;
            this.alternateMovementController.PreStart();
            
            this.alternateMovementController.PostStart();
        }

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            sprite = GetComponent<SpriteRenderer>();
            health.OnHealthChanged += UpdateHealth;
            health.OnHealthZero += () => { 
                OnDie?.Invoke(); GlobalGameData.SaveData(); 
                switcher.PlayGame(deathScene); 
            };

            #region Stamina
            stamina.OnStaminaChanged += () => { 
                staminaMask.sizeDelta = new Vector2((float)stamina.stamina / stamina.maxStamina * maxBarStamina, staminaMask.sizeDelta.y); 
            };

            stamina.OnStaminaZero += () => { 
                staminaExhaustedSound.Play(); 
            };

            stamina.OnStaminaMax += () => { 
                if (pauseStaminaRegen) pauseStaminaRegen = false; 
            };
            #endregion

            projectileDamage.Change(0);
            projectileSpeed.Change(0);
            projectileDuration.Change(0);
            projectileSpread.Change(0);
            projectilePiercing.Change(0);
            projectileCount.Change(0);
            fireRate.Change(0);
            staminaRegenDelay.Change(0);
            ChangeXP(0);
            StaminaTime();
            CalculateFireRate();
            SetMovementController(movementController, alternateMovementController);

            if (GlobalGameData.unlockedEquinox) {
                if (!GlobalGameData.unlockedEquinoxes[1]) OnDie += () => { GlobalGameData.unlockedEquinoxes[1] = true; };
                equinoxHandeler.gameObject.SetActive(true);
                equinoxHandeler.ChangeEquinox(EquinoxHandler.currentEquinox);
                switch (EquinoxHandler.currentEquinox) {
                    case 0: // Discord
                        equinoxDelay = 12.25f;
                        equinoxVector = UnityEngine.Random.insideUnitCircle * 6.5f;
                        equinoxIndicator.SetActive(true);
                        equinoxIndicator.transform.localPosition = (Vector3)equinoxVector;
                        break;
                    case 1: // Lament
                    case 2: // Redundant
                        break;
                }
                UpdateHook += EquinoxUpdate;
            }

            if (GlobalGameData.handSlot != null && GlobalGameData.handSlot.tags.Contains("sword")) {
                currentWeapon = WeaponType.Sword;
            } else {
                currentWeapon = WeaponType.MagicOne;
            }

            ApplyCorrectUpgrades();

            GlobalGameData.ClearPauseReasons();
            Cursor.visible = false;
        }

        public void ApplyCorrectUpgrades() {
            switch (currentWeapon) {
                case WeaponType.Sword:
                    projectileSpeed.GameObject.SetActive(false);
                    projectileDuration.GameObject.SetActive(false);
                    projectileSpread.GameObject.SetActive(false);
                    projectilePiercing.GameObject.SetActive(false);
                    projectileCount.GameObject.SetActive(false);
                    swordObject.SetActive(true);
                    spearObject.SetActive(false);
                    swordMouseObject.SetActive(true);
                    break;
                case WeaponType.Spear:
                    swordObject.SetActive(false);
                    spearObject.SetActive(true);
                    spearMouseObject.SetActive(true);
                    break;
                case WeaponType.Gun:
                    swordObject.SetActive(false);
                    spearObject.SetActive(false);
                    gunMouseObject.SetActive(true);
                    break;
                case WeaponType.MagicOne:
                    swordObject.SetActive(false);
                    spearObject.SetActive(false);
                    magicOneMouseObject.SetActive(true);
                    break;
            }
        }

        public void EquinoxUpdate() {
            switch (EquinoxHandler.currentEquinox) {
                case 0: // Discord
                    if (equinoxTimer >= equinoxDelay) {
                        equinoxTimer = 0f;
                        gameObject.transform.position += (Vector3)equinoxVector;
                        equinoxVector = UnityEngine.Random.insideUnitCircle * 6.5f;
                        equinoxIndicator.transform.localPosition = (Vector3)equinoxVector;
                    }
                    equinoxTimer += Time.deltaTime;
                    discordMarker.color = new Color(discordMarker.color.r, discordMarker.color.g, discordMarker.color.b, equinoxTimer / equinoxDelay);
                    break;
                case 1: // Lament
                    break;
                case 2: // Redundant
                    break;
            }
        }

        public void UpdateHealth(int amount) {
            float percent = (float)health.health / health.maxHealth;
            healthMask.sizeDelta = new Vector2(percent * maxBarHealth, healthMask.sizeDelta.y);
            hurtSound.Play();

            switch (EquinoxHandler.currentEquinox) {
                case 1: // Lament
                    if (percent <= 0.5f && UnityEngine.Random.Range(0f, 2f) > 1.75f) {
                        health.ChangeHealth((int)(-amount * 1.5f));
                        lamentSound.Play();
                    }
                    break;
            }
        }

        public void LeaveSettings() {
            settingsUI.SetActive(false);
            generalUI.SetActive(true);
            GlobalGameData.RemovePauseReason(settingsMenu);
            Cursor.visible = false;
        }

        public override void UpdateOther() {
            // If clicked e toggle the level up menu
            if (Input.GetKeyDown(KeyCode.E) && !commands.activeSelf) {
                if (levelUp.activeSelf) { 
                    levelUp.SetActive(false); 
                    GlobalGameData.RemovePauseReason(levelUpMenu);
                    Cursor.visible = false;
                } else { 
                    levelUp.SetActive(true); 
                    GlobalGameData.AddPauseReason(levelUpMenu);
                    if (notificationUI.activeSelf) { 
                        notificationUI.SetActive(false);
                    }
                    Cursor.visible = true;
                }
            } else if (Input.GetKeyDown(KeyCode.Escape) && !commands.activeSelf) {
                if (settingsUI.activeSelf) {
                    LeaveSettings();
                } else {
                    settingsUI.SetActive(true);
                    generalUI.SetActive(false);
                    GlobalGameData.AddPauseReason(settingsMenu);
                    Cursor.visible = true;
                }
            } else if (Input.GetKeyDown(KeyCode.Tab)) {
                if (commands.activeSelf) {
                    commands.SetActive(false);
                    GlobalGameData.RemovePauseReason(commandsMenu);
                    Cursor.visible = false;
                } else {
                    commands.SetActive(true);
                    GlobalGameData.AddPauseReason(commandsMenu);
                    Cursor.visible = true;
                }
            }
        }

        public override void UpdateAI() {
            base.UpdateAI();

            rb.angularVelocity = 0;

            if (GlobalGameData.moveMode) {
                alternateMovementController.HandleMovement(rb);
            } else { 
                movementController.HandleMovement(rb);
            }

            switch (currentWeapon) {
                case WeaponType.MagicOne:
                    HandleShooting();
                    break;
                case WeaponType.Sword:
                    HandleSwing();
                    break;
                case WeaponType.Spear:
                    Debug.Log("Unimplemented Spear");
                    //HandleStab();
                    break;
            }

            UpdateHook?.Invoke();
        }

        public void ChangeXP(int amount) {
            currentXP += amount;

            while (currentXP >= maxXP) { 
                currentXP -= maxXP;
                level++;
                levelText.text = level.ToString();
                XPIncrement(1);

                levelUpSound.Play();

                switch(level) {
                    case 2:
                        notificationUI.SetActive(true);
                        break;
                    default:
                        break;
                }

                if (level % 5 == 0) {
                    perkManager.BeginPerkChoice();
                }
            }

            float percent = (float)currentXP / maxXP;

            xpMask.sizeDelta = new Vector2(percent * maxBarXP, xpMask.sizeDelta.y);
        }

        private void HandleShooting() {
            if (delayTimer <= 0) {
                sprite.sprite = idleSprite;
                if (Input.GetMouseButton(0)) {
                    shootSound.Play();
                    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector2 direction = (Vector2)mousePosition - (Vector2)transform.position;
                    direction.Normalize();
                    
                    for (int i = 0; i < projectileCount.value; i++) {
                        // Calculate the angle for each projectile with spread
                        float spread = (projectileCount.value > 1) ? Mathf.Lerp(-projectileSpread.value, projectileSpread.value, (float)i / (projectileCount.value - 1)) : 0f;
                        float angleWithSpread = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + spread;

                        // Instantiate the projectile
                        GameObject clone = Instantiate(projectile, transform.position, Quaternion.identity);
                        clone.transform.parent = null;

                        // Calculate the new direction based on the spread angle
                        Vector2 spreadDirection = new(Mathf.Cos(angleWithSpread * Mathf.Deg2Rad), Mathf.Sin(angleWithSpread * Mathf.Deg2Rad));

                        // Set position and rotation
                        clone.transform.SetPositionAndRotation(transform.position + (Vector3)(spreadDirection * vec), Quaternion.Euler(new Vector3(0, 0, angleWithSpread)));

                        // Set the projectile velocity
                        clone.GetComponent<Rigidbody2D>().velocity = spreadDirection * projectileSpeed.value;

                        // Initialize projectile properties
                        clone.GetComponent<Projectile>().InitStuff(projectileSpeed.value, projectileDamage.value, 1 + 0.5f * projectileDuration.value, projectilePiercing.value);
                    }
                    delayTimer = fireRateUsed;
                }
            } else {
                sprite.sprite = attackSprite;
                delayTimer -= Time.deltaTime;
            }
        }

        private void HandleSwing() {
            if (delayTimer <= 0) {
                sprite.sprite = idleSprite;
                if (Input.GetMouseButton(0)) {
                    swingSound.Play();
                    
                    swordObject.GetComponent<RotateController>().Rotate(fireRateUsed * 0.75f);

                    if (GlobalGameData.fingerSlot != null && GlobalGameData.fingerSlot.name == "RedCard") {
                        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        Vector2 direction = (Vector2)mousePosition - (Vector2)transform.position;
                        direction.Normalize();
                        
                        for (int i = 0; i < 5; i++) {
                            // Calculate the angle for each projectile with spread
                            float spread = Mathf.Lerp(-projectileSpread.value, projectileSpread.value, (float)i / (projectileCount.value - 1));
                            float angleWithSpread = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + spread;

                            // Instantiate the projectile
                            GameObject clone = Instantiate(redCard, transform.position, Quaternion.identity);
                            clone.transform.parent = null;

                            // Calculate the new direction based on the spread angle
                            Vector2 spreadDirection = new(Mathf.Cos(angleWithSpread * Mathf.Deg2Rad), Mathf.Sin(angleWithSpread * Mathf.Deg2Rad));

                            // Set position and rotation
                            clone.transform.SetPositionAndRotation(transform.position + (Vector3)(spreadDirection * vec), Quaternion.Euler(new Vector3(0, 0, angleWithSpread)));

                            // Set the projectile velocity
                            clone.GetComponent<Rigidbody2D>().velocity = spreadDirection * projectileSpeed.value;

                            // Initialize projectile properties
                            clone.GetComponent<Projectile>().InitStuff(1.5f, projectileDamage.value, 3.5f, 3);
                        }
                    }

                    delayTimer = fireRateUsed;
                }
            } else {
                sprite.sprite = attackSprite;
                delayTimer -= Time.deltaTime;
            }
        }

        private IEnumerator PauseStaminaRegen() {
            pauseStaminaRegen = true;
            yield return new WaitForSeconds(staminaRegenDelay.value);
            stamina.ChangeStamina(stamina.maxStamina);
            pauseStaminaRegen = false;
        }

        public void StaminaTime() {
            if (!Input.GetKey(KeyCode.LeftShift)) stamina.ChangeStamina(1);
            StartCoroutine(StaminaCoroutine());
        }

        private IEnumerator StaminaCoroutine() {
            yield return new WaitForSeconds(1f / staminaRegenDelay.value);
            StaminaTime();
        }

        public void BoostSpeed(float amount, float duration) {
            StartCoroutine(BoostSpeedCoroutine(amount, duration));
        }

        private IEnumerator BoostSpeedCoroutine(float amount, float duration) {
            movementController.tempSpeed += amount;
            yield return new WaitForSeconds(duration);
            movementController.tempSpeed -= amount;
        }

        void OnTriggerEnter2D(Collider2D other) {
            if (GlobalGameData.isPaused) return;
            
            if (Time.time < nextInvulnerabilityTime) {
                return;
            }

            if (other.gameObject.CompareTag("Projectile") && other.gameObject.TryGetComponent<Projectile>(out var projectile) && projectile.owner == Owner.Enemy) {
                projectile.Hit();
                foreach (StatusEffectData effect in projectile.effects) {
                    AddStatusEffect(effect);
                }
                DirectDamage((int)-projectile.damage);
            }
        }
    }

    public enum WeaponType {
        Gun,
        MagicOne,
        Sword,
        Spear
    }

    public interface IValueStat {
    }

    [Serializable]
    public struct FloatStat : IValueStat {
        public string format;
        public TMP_Text text;
        public float value;
        public GameObject GameObject;

        public void Change(float amount) { 
            value += amount; 
            text.text = format + value;
        }

        public void Set(float amount) {
            value = amount;
            text.text = format + value;
        }
    }

    [Serializable]
    public struct IntStat : IValueStat {
        public string format;
        public TMP_Text text;
        public int value;
        public GameObject GameObject;

        public void Change(int amount) { 
            value += amount; 
            text.text = format + value;
        }

        public void Set(int amount) {
            value = amount;
            text.text = format + value;
        }
    }
}