using UnityEngine;
using ASimpleRoguelike.Equinox;
using ASimpleRoguelike.Inventory;

namespace ASimpleRoguelike {
    public class MenuExtra : MonoBehaviour
    {
        public PerkData[] perks;
        public EquinoxSetup equinoxSetup;
        public FactionSetup factionSetup;
        public ItemSetup itemSetup;

        void Start()
        {
            foreach (var perk in perks) 
                PerkData.perks.Add(perk);
            
            equinoxSetup.Setup();
            factionSetup.Setup();
            itemSetup.Setup();
            GlobalGameData.NewData();

            GlobalGameData.LoadData();
        }
    }
}