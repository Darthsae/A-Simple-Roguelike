using UnityEngine;
using ASimpleRoguelike.Equinox;
using ASimpleRoguelike.Inventory;
using ASimpleRoguelike.Map;
using UnityEngine.Audio;
using ASimpleRoguelike.Perk;

namespace ASimpleRoguelike {
    public class MenuExtra : MonoBehaviour {
        public PerkData[] perks;
        public EquinoxSetup equinoxSetup;
        public FactionSetup factionSetup;
        public ItemSetup itemSetup;
        public MapSetup mapSetup;
        public AudioMixer audioMixer;
        public string[] audioMixers;

        void Start() {
            if (!Logger.open) {
                Logger.StartLogging();
                Logger.LogInfo("Started logging");
            }

            foreach (var perk in perks) 
                PerkData.perks.Add(perk);
            
            GlobalGameData.audioMixer = audioMixer;

            foreach (var audioMixer in audioMixers) {
                GlobalGameData.audioMixerGroups.Add(audioMixer);
            }
            
            equinoxSetup.Setup();
            factionSetup.Setup();
            itemSetup.Setup();
            mapSetup.Setup();
            GlobalGameData.NewData();

            GlobalGameData.LoadData();

            Cursor.visible = true;

            PerkManager.godsAttention = GodState.NONE;
        }
    }
}