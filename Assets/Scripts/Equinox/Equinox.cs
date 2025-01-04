using System.Collections.Generic;
using UnityEngine;

namespace ASimpleRoguelike.Equinox {
    public class Equinox : MonoBehaviour {
        public static List<EquinoxData> equinoxes;
        public static int EquinoxCount => equinoxes.Count;
        public EquinoxData equinox;
    }

    [System.Serializable]
    public enum EquinoxAttunement {
        Ethereal,
        Celestial,
        Divine,
        Deathless
    }
}