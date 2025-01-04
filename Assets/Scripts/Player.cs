using System;
using System.Collections;
using TMPro;
using UnityEngine;
using ASimpleRoguelike.Equinox;
using System.Linq;

namespace ASimpleRoguelike {
    public class Player : Entity.Entity
    {
        public static Vector2 vec = new(1f, 1f);
        public GameObject levelUp;
        public RectTransform xpMask;
        public RectTransform healthMask;
        public PerkManager perkManager;

        public SpriteRenderer sprite;

        public GameObject commands;

        public WeaponType currentWeapon = WeaponType.MagicOne; 

        #region 
        public GameObject swordObject;
        public GameObject spearObject;
        #endregion

        #region Indicator Objects
        public GameObject rushIndicator;
        public GameObject rushMover;
        public GameObject equinoxIndicator;
        #endregion

        #region Textures
        public Sprite idleSprite;
        public Sprite attackSprite;
        #endregion

        #region Rush
        public bool isRushing = false;
        public bool isRushingEnabled = true;
        public float russianSpeed;
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
        public AudioSource moveSound;
        public AudioSource shootSound;
        
        public AudioSource levelUpSound;
        public AudioSource staminaExhaustedSound;

        public AudioSource lamentSound;
        #endregion

        public event Action OnDie;

        private float delayTimer = 1f;
        private float stamTimer = 0.0f;
        public float stamDelay = 0.5f;
        

        #region Equinox
        private float equinoxTimer = 0.0f;
        public float equinoxDelay = 0.5f;
        public Vector2 equinoxVector;
        #endregion

        public float tempSpeed = 0f;

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
        public FloatStat speed;
        public IntStat staminaRegenDelay;
        public IntStat rushSpeed;

        public IntStat projectileDamage;
        public FloatStat projectileSpeed;
        public FloatStat projectileDuration;
        public FloatStat projectileSpread;
        public IntStat projectilePiercing;
        public IntStat projectileCount;

        public void CalculateFireRate() {
            fireRateUsed = fireRateDefault - (0.01f * fireRate.value);
        }


        public void CalculateRushSpeed() {
            russianSpeed = 1.0f * (2.5f + (0.25f * rushSpeed.value));
        }

        public void XPIncrement(int amount) {
            spendXP += amount;
            spendXPText.text = spendXP.ToString();
        }

        public void ProjectileDamage(int amount) {if (spendXP > 0) { projectileDamage.Change(amount); swordObject.GetComponent<HarmingArea>().damage += amount; XPIncrement(-1);}}
        public void ProjectileSpeed(float amount) {if (spendXP > 0) { projectileSpeed.Change(amount); XPIncrement(-1); }}
        public void RushSpeed(int amount) {if (spendXP > 0) { rushSpeed.Change(amount); CalculateRushSpeed(); XPIncrement(-1);}}
        public void ProjectileDuration(float amount) {if (spendXP > 0) { projectileDuration.Change(amount); XPIncrement(-1); }}
        public void ProjectileSpread(float amount) {if (spendXP > 0) { projectileSpread.Change(amount); XPIncrement(-1); }}
        public void ProjectilePiercing(int amount) {if (spendXP > 0) { projectilePiercing.Change(amount); XPIncrement(-1); }}
        public void ProjectileCount(int amount) {if (spendXP > 0) { projectileCount.Change(amount); XPIncrement(-1); }}
        public void Speed(float amount) {if (spendXP > 0) { speed.Change(amount); XPIncrement(-1); }}
        public void FireRate(int amount) {if (spendXP > 0) { fireRate.Change(amount); CalculateFireRate(); XPIncrement(-1); }}
        public void StaminaRegenDelay(int amount) {if (spendXP > 0) { staminaRegenDelay.Change(amount); XPIncrement(-1); }}

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            sprite = GetComponent<SpriteRenderer>();
            health.OnHealthChanged += UpdateHealth;
            health.OnHealthZero += () => { 
                OnDie?.Invoke(); GlobalGameData.SaveData(); 
                switcher.PlayGame(deathScene); 
            };
            stamina.OnStaminaChanged += () => { 
                staminaMask.sizeDelta = new Vector2((float)stamina.stamina / stamina.maxStamina * maxBarStamina, staminaMask.sizeDelta.y); 
            };
            stamina.OnStaminaZero += () => { staminaExhaustedSound.Play(); };
            stamina.OnStaminaMax += () => { if (pauseStaminaRegen) pauseStaminaRegen = false; };
            projectileDamage.Change(0);
            projectileSpeed.Change(0);
            projectileDuration.Change(0);
            projectileSpread.Change(0);
            projectilePiercing.Change(0);
            projectileCount.Change(0);
            speed.Change(0);
            fireRate.Change(0);
            staminaRegenDelay.Change(0);
            rushSpeed.Change(0);
            ChangeXP(0);
            StaminaTime();
            CalculateFireRate();
            CalculateRushSpeed();

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
                    break;
                case WeaponType.Spear:
                    swordObject.SetActive(false);
                    spearObject.SetActive(true);
                    break;
                case WeaponType.Gun:
                    swordObject.SetActive(false);
                    spearObject.SetActive(false);
                    break;
                case WeaponType.MagicOne:
                    swordObject.SetActive(false);
                    spearObject.SetActive(false);
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
            GlobalGameData.isPaused = false;
        }

        private void Update()
        {
            // If clicked e toggle the level up menu
            if (Input.GetKeyDown(KeyCode.E) && !commands.activeSelf) {
                if (levelUp.activeSelf) { levelUp.SetActive(false); GlobalGameData.isPaused = false; }
                else { 
                    levelUp.SetActive(true); 
                    GlobalGameData.isPaused = true; 
                    if (notificationUI.activeSelf) { notificationUI.SetActive(false);}
                }
            }
            else if (Input.GetKeyDown(KeyCode.Escape) && !commands.activeSelf) {
                if (settingsUI.activeSelf) {
                    LeaveSettings();
                } else {
                    settingsUI.SetActive(true);
                    generalUI.SetActive(false);
                    GlobalGameData.isPaused = true;
                }
            } 
            else if (Input.GetKeyDown(KeyCode.Tab)) {
                if (commands.activeSelf) {
                    commands.SetActive(false);
                    GlobalGameData.isPaused = false;
                } else {
                    commands.SetActive(true);
                    GlobalGameData.isPaused = true;
                }
            }

            rb.angularVelocity = 0;

            if (GlobalGameData.isPaused) {
                rb.velocity = Vector2.zero;
                return;
            }

            HandleMovement();
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

        private void HandleMovement()
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveY = Input.GetAxis("Vertical");

            Vector2 movement = new(moveX, moveY);
            movement.Normalize();
            rb.velocity = (speed.value + tempSpeed) * movement;

            float tempPitch = movement.magnitude;

            if (movement != Vector2.zero)
            {
                if (GlobalGameData.neckSlot != null && GlobalGameData.neckSlot.name == "DiscordantPendant") {
                    if (Input.GetKeyDown(KeyCode.LeftControl)) {
                        isRushing = true;
                        rushIndicator.SetActive(true);
                        rushMover.SetActive(true);
                        rushMover.transform.localPosition = Vector3.zero;
                    } else if (Input.GetKeyUp(KeyCode.LeftControl)) {
                        isRushing = false;
                        transform.position = rushMover.transform.position;
                        rushIndicator.SetActive(false);
                        rushMover.SetActive(false);
                    }
                } else {
                    if (Input.GetKey(KeyCode.LeftShift) && stamina.stamina > 0 && !pauseStaminaRegen) {
                        rb.velocity *= 1.5f; 

                        tempPitch *= 1.5f;

                        if (stamTimer > 0) stamTimer -= Time.deltaTime;
                        else {
                            stamina.ChangeStamina(-1);
                            stamTimer = stamDelay;
                        }
                    }
                    else if (pauseStaminaRegen) {
                        rb.velocity *= 0.9f;
                    }
                }
                
                moveSound.pitch = tempPitch; 
                if (!moveSound.isPlaying) { moveSound.Play(); }
                float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }

            if (isRushing) {
                Debug.Log("Rushing: " + rb.velocity.magnitude + " * " + russianSpeed + " = " + rb.velocity.magnitude * russianSpeed);
                rushIndicator.transform.localPosition = rb.velocity * russianSpeed;
            }
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
            }
            else
            {
                sprite.sprite = attackSprite;
                delayTimer -= Time.deltaTime;
            }
        }

        private void HandleSwing() {
            if (delayTimer <= 0) {
                sprite.sprite = idleSprite;
                if (Input.GetMouseButton(0)) {
                    shootSound.Play();
                    
                    swordObject.GetComponent<RotateController>().Rotate(fireRateUsed * 0.75f);
                    delayTimer = fireRateUsed;
                }
            }
            else
            {
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

        public void BoostSpeed(float amount, float duration)
        {
            StartCoroutine(BoostSpeedCoroutine(amount, duration));
        }

        private IEnumerator BoostSpeedCoroutine(float amount, float duration)
        {
            tempSpeed += amount;
            yield return new WaitForSeconds(duration);
            tempSpeed -= amount;
        }

        void OnTriggerEnter2D(Collider2D other) {
            if (GlobalGameData.isPaused) return;
            
            if (Time.time < nextInvulnerabilityTime) {
                return;
            }

            if (other.gameObject.CompareTag("Projectile") && other.gameObject.TryGetComponent<Projectile>(out var projectile) && projectile.owner == Owner.Enemy) {
                projectile.Hit();
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
    public struct FloatStat : IValueStat
    {
        public string format;
        public TMP_Text text;
        public float value;
        public GameObject GameObject;

        public void Change(float amount) { 
            value += amount; 
            text.text = format + value;
        }
    }

    [Serializable]
    public struct IntStat : IValueStat
    {
        public string format;
        public TMP_Text text;
        public int value;
        public GameObject GameObject;

        public void Change(int amount) { 
            value += amount; 
            text.text = format + value;
        }
    }
}