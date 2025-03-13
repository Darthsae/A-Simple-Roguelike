using UnityEngine;
using ASimpleRoguelike.Equinox;
using ASimpleRoguelike.Inventory;
using UnityEngine.Audio;

namespace ASimpleRoguelike {
    public class MenuExtra : MonoBehaviour
    {
        public PerkData[] perks;
        public EquinoxSetup equinoxSetup;
        public FactionSetup factionSetup;
        public ItemSetup itemSetup;
        public AudioMixer audioMixer;
        public string[] audioMixers;

        void Start()
        {
            foreach (var perk in perks) 
                PerkData.perks.Add(perk);
            
            GlobalGameData.audioMixer = audioMixer;

            foreach (var audioMixer in audioMixers) {
                GlobalGameData.audioMixerGroups.Add(audioMixer);
            }
            
            equinoxSetup.Setup();
            factionSetup.Setup();
            itemSetup.Setup();
            GlobalGameData.NewData();

            GlobalGameData.LoadData();

            Cursor.visible = true;
        }
    }
}