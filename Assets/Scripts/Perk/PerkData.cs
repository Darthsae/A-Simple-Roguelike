using System.Collections.Generic;
using UnityEngine;

namespace ASimpleRoguelike.Perk {
    [CreateAssetMenu(menuName = "A Simple Roguelike/PerkData")]
    public class PerkData : ScriptableObject {
        public static HashSet<PerkData> perks = new();

        [Tooltip("Dependenant perks that need to be unlocked before this perk can be unlocked")]
        public PerkWithLevel[] prereqs;

        [Tooltip("The sprite to use for this perk, 32x32 px")]
        public Sprite sprite;
        
        [Tooltip("The description of the perk")]
        public string description;

        [Tooltip("The update type of the perk, used to reduce code redundancy")]
        public UpdateType updateType = UpdateType.None;
        [Tooltip("The kill type of the perk, used to reduce code redundancy")]
        public KillType onKill = KillType.None;

        [Tooltip("Max level of the perk")]
        public int maxLevel = 1;

        [Tooltip("Min phase for the perk to appear")]
        public int minPhase = 0;

        [Tooltip("Max phase for the perk to appear")]
        public int maxPhase = int.MaxValue;

        [Tooltip("The weight of the perk")]
        public float weight = 1.0f;

        [Tooltip("Consecutive timers for the perk")]
        public float[] timers = new float[0];

        public bool CanUse(List<PerkWithLevel> unlockedPerks, int phase) {
            if (phase < minPhase || phase > maxPhase) {
                return false;
            }
            foreach (PerkWithLevel perk in prereqs) {
                if (unlockedPerks.FindIndex((perk_) => perk_.perk == perk.perk && perk_.level >= perk.level) == -1) {
                    return false;
                }
            }
            if (unlockedPerks.FindIndex((perk) => perk.perk == this && perk.level >= maxLevel) != -1) {
                return false;
            }
            return true;
        }

        public static PerkData GetRandomValidPerk(List<PerkWithLevel> unlockedPerks, int phase, List<PerkData> displayedPerks) {
            float totalWeight = 0.0f;

            List<PerkData> perkables = new();

            foreach (var perk in perks) {
                if (perk.CanUse(unlockedPerks, phase) && !displayedPerks.Contains(perk)) {
                    perkables.Add(perk);
                    totalWeight += perk.weight;
                }
            }

            if (totalWeight == 0.0f) {
                return null;
            }

            float random = Random.Range(0.0f, totalWeight);

            foreach (var perk in perkables) {
                if (random < perk.weight) {
                    return perks.TryGetValue(perk, out PerkData perkData) ? perkData : null;
                } else {
                    random -= perk.weight;
                }
            }

            return null;
        }
    }

    [System.Serializable]
    public class PerkWithLevel {
        public PerkData perk;
        public int level;

        public PerkWithLevel(PerkData perk, int level) {
            this.perk = perk;
            this.level = level;
        }
    }

    public enum UpdateType {
        None,
        Regeneration,
        DamageZone,
        Custom
    }

    public enum KillType {
        None,
        LifeSteal,
        Custom
    }
}