using UnityEngine;

namespace ASimpleRoguelike {
    public class ProjectileShooter : MonoBehaviour {
        public float delay = 2.5f;
        public float timer = 0.0f;
        public GameObject prefab;

        public float speed = 2f;
        public float time = 10f;
        public bool loop = true;
        public int amount = 5;
        public int damage = 25;
        public int piercing = 1;
        public Owner owner = Owner.Enemy;
        public float offset = 2.5f;
        public AudioSource fireSound;

        private void Update() {
            if (GlobalGameData.isPaused) {
                return;
            }

            if (timer >= 0f) {
                timer -= Time.deltaTime;
            } else {
                timer = delay;

                for (int i = 0; i < amount; i++) {
                    float angle = i * (360f / amount) * Mathf.Deg2Rad;
                    GameObject clone = Instantiate(prefab, transform.position + new Vector3(Mathf.Cos(angle) * offset, Mathf.Sin(angle) * offset, 0f), Quaternion.identity);
                    clone.transform.parent = null;
                    clone.GetComponent<Projectile>().InitStuff(speed, damage, time, piercing, owner);
                    // Set rotation respecting angle
                    clone.transform.rotation = Quaternion.Euler(0f, 0f, angle * Mathf.Rad2Deg);
                }

                if (fireSound != null) {
                    fireSound.Play();
                }
            }
        }
    }
}