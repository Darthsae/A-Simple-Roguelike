using UnityEngine;

namespace ASimpleRoguelike {
    public class Growth : MonoBehaviour
    {
        public float amount;

        void Update()
        {
            if (GlobalGameData.isPaused) return;
            transform.localScale += amount * Time.deltaTime * Vector3.one;
        }
    }
}