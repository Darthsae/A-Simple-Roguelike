using System;
using System.Collections.Generic;
using UnityEngine;

namespace ASimpleRoguelike {
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(AnimatibleImage))]
    public class DamagedAnimatibleSprite : MonoBehaviour {
        public List<ListHolder<SpriteHolder>> sprites = new();

        void Start () {
            Health health = GetComponent<Health>();
            health.OnHealthChanged += (int healthChange) => {
                float percent = (float)health.health / health.maxHealth;
                int index = Mathf.FloorToInt(percent * sprites.Count);
                if (index < 0) index = 0;
                if (index >= sprites.Count) index = sprites.Count - 1;
                GetComponent<AnimatibleImage>().sprites = sprites[index].list;
                GetComponent<AnimatibleImage>().ApplyCurrent();
            };
        }
    }

    [Serializable]
    public struct ListHolder<T> {
        public List<T> list;
    }
}
