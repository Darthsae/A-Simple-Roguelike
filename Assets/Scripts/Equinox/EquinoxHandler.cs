using System.Collections.Generic;
using UnityEngine;

namespace ASimpleRoguelike.Equinox {
    public class EquinoxHandler : MonoBehaviour
    {
        public List<Equinox> equinoxes;
        public static int currentEquinox = -1;

        public int equinoxMaxHeight = 300;
        public RectTransform equinoxMask;

        public void ChangeEquinox(int equinox) {
            if (equinox > equinoxes.Count - 1 || equinox < 0 || !GlobalGameData.unlockedEquinoxes[equinox]) return;

            if (currentEquinox != -1) {
                equinoxes[currentEquinox].gameObject.SetActive(false);
            }

            if (equinox != -1) {
                equinoxes[equinox].gameObject.SetActive(true);
            }
        }
    }
}