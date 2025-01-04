using System.Collections.Generic;
using UnityEngine;

namespace ASimpleRoguelike {
    public class Faction : MonoBehaviour
    {
        public static List<FactionData> factions;
        public static int FactionCount => factions.Count;
        public FactionData faction;

        public static string FactionNamePair(FactionData factionOne, FactionData factionTwo) {
            List<string> list = new(){factionOne.name, factionTwo.name};
            list.Sort();
            return list[0] + ":" + list[1];
        }
    }
}