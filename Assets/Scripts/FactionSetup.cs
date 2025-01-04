using System.Collections.Generic;
using UnityEngine;

namespace ASimpleRoguelike {
    public class FactionSetup : MonoBehaviour
    {
        public List<FactionData> factions;

        public void Setup()
        {
            Faction.factions = new List<FactionData>();

            foreach (var faction in factions)
            {
                Faction.factions.Add(faction);
            }
        }
    }
}