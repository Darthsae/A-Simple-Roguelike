using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASimpleRoguelike {
    public class AnimatibleImage : MonoBehaviour {
        public List<SpriteHolder> sprites;

        public SpriteRenderer image;

        public int index = -1;

        public void Start() {
            StartCoroutine(ChangeSprite());
        }

        IEnumerator ChangeSprite() {
            while (true) {
                index = (index + 1) % sprites.Count;
                image.sprite = sprites[index].sprite;
                yield return new WaitForSeconds(sprites[index].time);
            }
        }
    }
}