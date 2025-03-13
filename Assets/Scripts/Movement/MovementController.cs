using UnityEngine;

namespace ASimpleRoguelike.Movement {
    public abstract class MovementController : MonoBehaviour {
        public string displayName = "Movement Controller";

        public Player player;
        public float tempSpeed = 0f;
        
        public AudioSource moveSound;
        
        public void Speed(float amount) {if (player.spendXP > 0) { speed.Change(amount); player.XPIncrement(-1); }}
        
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

        public void RushSpeed(int amount) {if (player.spendXP > 0) { rushSpeed.Change(amount); CalculateRushSpeed(); player.XPIncrement(-1);}}
        #endregion
        
        public void PreStart() {
            speed.Change(0);
            rushSpeed.Change(0);
        }

        public void PostStart() {
            CalculateRushSpeed();
        }

        public abstract void HandleMovement(Rigidbody2D rigidbody);
    }
}