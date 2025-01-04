using System.Collections.Generic;
using UnityEngine;

namespace ASimpleRoguelike.Equinox {
    public class EquinoxSetup : MonoBehaviour
    {
        public List<EquinoxData> equinoxes;

        public void Setup()
        {
            Equinox.equinoxes = new List<EquinoxData>();

            foreach (var equinox in equinoxes)
            {
                Equinox.equinoxes.Add(equinox);
            }
        }
    }
}