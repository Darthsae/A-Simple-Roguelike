using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace ASimpleRoguelike {
    public class AnimatibleSprite : MonoBehaviour {
        public List<SpriteHolder> sprites;

        public Image image;

        public int index = -1;

        public void Start() {
            StartCoroutine(ChangeSprite());
        }

        IEnumerator ChangeSprite() {
            while (true) {
                index = (index + 1) % sprites.Count;
                image.sprite = sprites[index].sprite;
                image.gameObject.GetComponent<RectTransform>().localScale = sprites[index].scale;
                yield return new WaitForSeconds(sprites[index].time);
            }
        }
    }

    [System.Serializable]
    public class SpriteHolder {
        public Vector2 scale;
        public Sprite sprite;
        public float time;
    }
}