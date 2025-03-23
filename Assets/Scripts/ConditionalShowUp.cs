using UnityEngine;

namespace ASimpleRoguelike {
    public class ConditionalShowUp : MonoBehaviour {
        public ConditionType conditionType;

        // Start is called before the first frame update
        void Start() {
            switch (conditionType) {
                case ConditionType.EquinoxUnlocked:
                    gameObject.SetActive(GlobalGameData.unlockedEquinox);
                    break;
            }
        }
    }

    [System.Serializable]
    public enum ConditionType {
        EquinoxUnlocked
    }
}