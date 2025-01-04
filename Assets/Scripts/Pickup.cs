using UnityEngine;

namespace ASimpleRoguelike {
    public class Pickup : MonoBehaviour
    {
        //For when the player picks it up.
        public PickupType pickupType;

        public void Init(PickupType type) {
            pickupType = type;
            GetComponent<SpriteRenderer>().sprite = GameObject.Find("GlobalHolder").GetComponent<GlobalGameData>().barrelSprites[(int)type];
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (GlobalGameData.isPaused) return;
            
            if (other.CompareTag("Player"))
            {
                HandlePickup(other.gameObject);
                Destroy(gameObject); // Destroy the pickup after it's picked up
            }
        }

        private void HandlePickup(GameObject player)
        {
            switch (pickupType)
            {
                case PickupType.Health:
                    // Handle health pickup
                    player.GetComponent<Health>().ChangeHealth(10); // Example logic
                    break;
                case PickupType.SpeedBoost:
                    // Handle speed boost pickup
                    player.GetComponent<Player>().BoostSpeed(5f, 10f); // Example logic
                    break;
            }
        }
    }

    [System.Serializable]
    public enum PickupType
    {
        Health,
        SpeedBoost,
        PlaceHolder3,
        PlaceHolder4,
        PlaceHolder5,
        PlaceHolder6,
        PlaceHolder7,
        PlaceHolder8,
        PlaceHolder9
    }
}