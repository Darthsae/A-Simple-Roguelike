using System.Collections.Generic;
using UnityEngine;

namespace ASimpleRoguelike.Entity {
    [RequireComponent(typeof(Enemy))]
    public class WormHandler : MonoBehaviour {
        public Enemy enemy;
        public GameObject bodyPrefab;
        public List<Sprite> bodySprites;
        public Sprite tailSprite;
        public int segmentCount = 5;
        public List<Worm> segments = new();

        private void Start() {
            enemy = GetComponent<Enemy>();

            for (int i = 0; i < segmentCount; i++) {
                GameObject segment = Instantiate(bodyPrefab, transform.position, transform.rotation);
                Worm worm = segment.GetComponent<Worm>();
                worm.sprite.sprite = bodySprites[Random.Range(0, bodySprites.Count)];
                worm.leaderTransform = (i == 0) ? transform : segments[i - 1].transform;
                worm.leader = enemy;
                worm.Init();
                segments.Add(worm);
            }

            segments[segmentCount - 1].sprite.sprite = tailSprite;
        }
    }
}