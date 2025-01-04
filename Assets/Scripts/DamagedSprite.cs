using System.Collections.Generic;
using UnityEngine;

namespace ASimpleRoguelike {
    [RequireComponent(typeof(Health))]
    public class DamagedSprite : MonoBehaviour
    {
        public List<Sprite> sprites = new();

        void Start () {
            Health health = GetComponent<Health>();
            health.OnHealthChanged += (int healthChange) => {
                float percent = (float)health.health / health.maxHealth;
                int index = Mathf.FloorToInt(percent * sprites.Count);
                if (index < 0) index = 0;
                if (index >= sprites.Count) index = sprites.Count - 1;
                GetComponent<SpriteRenderer>().sprite = sprites[index];
            };
        }
    }
}
