using UnityEngine;

namespace ASimpleRoguelike.Movement {
    public abstract class MovementController : MonoBehaviour {
        public string displayName = "Movement Controller";

        public Player player;
        public float tempSpeed = 0f;
        
        public AudioSource moveSound;
        
        public void Speed(float amount) {speed.Change(amount);}
        
        public FloatStat speed;
        public IntStat rushSpeed;

        #region Rush
        public bool isRushing = false;
        public bool isRushingEnabled = true;
        public float russianSpeed;

        public GameObject rushIndicator;
        public GameObject rushMover;
        
        public void CalculateRushSpeed() {
            russianSpeed = 1.0f * (2.5f + (0.25f * rushSpeed.value));
        }

        public void RushSpeed(int amount) {rushSpeed.Change(amount); CalculateRushSpeed();}
        #endregion
        
        public void PreStart() {
            speed.Change(0);
            rushSpeed.Change(0);

            if (GlobalGameData.neckSlot == null || GlobalGameData.neckSlot.name != "DiscordantPendant") {
                rushSpeed.GameObject.SetActive(false);
            }
        }

        public void PostStart() {
            CalculateRushSpeed();
        }

        public abstract void HandleMovement(Rigidbody2D rigidbody);
    }
}