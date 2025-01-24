using System.Collections.Generic;
using UnityEngine;
using ASimpleRoguelike.Entity;

namespace ASimpleRoguelike {
    [System.Serializable]
    public enum GodState {
        NONE,
        NOTICED,
        OBSERVED,
        INCURSED,
        ALL
    }

    public class PerkManager : MonoBehaviour
    {
        [ContextMenu("Add Perk")]
        void AddPerk()
        {
            BeginPerkChoice();
        }

        public List<StringToInt> perkNumbers = new();

        public static bool poppedTemporal = false;
        public static GodState godsAttention = GodState.NONE;
        public static int spawnerMaxMultiplier = 1;

        public int thrallCount = 0;

        [Header("These are util stuff for zones")]
        public GameObject healthZone;
        public GameObject speedZone;

        public Player player;
        public List<PerkWithLevel> unlockedPerks = new();

        public List<PerkCardDisplay> perkCards = new();

        public List<RotationHolder> spawnedRotating = new();

        public TimerHandler timer;

        public GameObject graveThrall;

        public GameObject perkChoiceUI;
        public GameObject perkDisplayUI;

        public GameObject lightningBolt;
        
        public GameObject mace;

        public List<PerkTimerHolder> times = new();
        public List<string> onKills;

        public void OpenPerkDisplayUI() {
            foreach (PerkWithLevel perk in unlockedPerks) {
                Debug.Log(perk.perk.name + " " + perk.level);
            }

            perkDisplayUI.SetActive(true);
        }

        public void ClosePerkDisplayUI() {
            perkDisplayUI.SetActive(false);
        }

        public void BeginPerkChoice() {
            GlobalGameData.isPaused = true;
            List<PerkData> perks = new();
            int temper = 0;
            for (int i = 0; i < perkCards.Count; i++) {
                PerkData perkOption = PerkData.GetRandomValidPerk(unlockedPerks, timer.GetPhase(), perks);

                if (perkOption == null) break;

                temper++;
            
                perkCards[i].SetData(perkOption, GetPerkCount(perkOption) + 1);
                perkCards[i].gameObject.SetActive(true);

                perks.Add(perkOption);
            }

            if (temper == 0) {
                GlobalGameData.isPaused = false;
                ClosePerkDisplayUI();
            } else {
                perkChoiceUI.SetActive(true);
            }
        }

        public int GetPerkCount(PerkData perkData) {
            int index = unlockedPerks.FindIndex((perk) => perk.perk == perkData);
            return index == -1 ? 0 : unlockedPerks[index].level;
        }

        public void SelectedPerk(PerkData perkData1) {
            perkChoiceUI.SetActive(false);

            PerkData.perks.TryGetValue(perkData1, out PerkData perkData);

            int index = unlockedPerks.FindIndex((perk) => perk.perk == perkData);
            if (index == -1) { 
                unlockedPerks.Add(new PerkWithLevel(perkData, 1));
                if (perkData.updateType != UpdateType.None) {
                    times.Add(new PerkTimerHolder() {
                        time = perkData.timers[0],
                        timeOnSet = perkData.timers[0],
                        perkName = perkData.name
                    });
                    Debug.Log(perkData.name + " " + perkData.timers[0]);
                }

                if (perkData.onKill != KillType.None) {
                    onKills.Add(perkData.name);
                }

                switch (perkData.name) {
                    case "Reaper":
                        for (int i = 0; i < 3; i++) {
                            AddRotating(Instantiate(spawnedRotating[0].prefab, player.transform.position, Quaternion.identity), 0);
                        }
                        break;
                    case "As Above So Below":
                        godsAttention = GodState.NOTICED;
                        break;
                    case "Stalwart":
                        player.health.SetMaxHealth(player.health.maxHealth * 2);
                        break;
                    case "Temporal Backlash":
                        player.health.PreHealthZero += (int health, out int newHealth) => {
                            newHealth = 0;
                            if (poppedTemporal) 
                                return true;
                            poppedTemporal = true;
                            newHealth = 50;

                            unlockedPerks.Clear();
                            unlockedPerks.Add(new PerkWithLevel(perkData, 1));
                            onKills.Clear();
                            perkNumbers.Clear();
                            times.Clear();

                            for (int i = 0; i < spawnedRotating.Count; i++) {
                                foreach (GameObject obj in spawnedRotating[i].spawnedRotating) {
                                    Destroy(obj);
                                }
                                spawnedRotating[i].spawnedRotating.Clear();
                            }

                            player.health.SetMaxHealth(100);

                            return false;
                        };
                        break;
                    case "Fateweavers Gamble":
                        BeginPerkChoice();
                        return;
                    case "Bone Shield":
                        perkNumbers.Add(new StringToInt("Bone Shield", 3));
                        break;
                    case "Mace of the Gods":
                        mace.SetActive(true);
                        break;
                }
            } else {
                unlockedPerks[index].level++;
                switch (perkData.name) {
                    case "Bone Shield":
                        perkNumbers.Find((stringToInt) => stringToInt.perkName == "Bone Shield").number += 1;
                        break;
                    case "Mace of the Gods":
                        mace.GetComponent<ChainRotater>().NewChild();
                        break;
                }
            }

            foreach (PerkCardDisplay card in perkCards) {
                card.gameObject.SetActive(false);
            }

            GlobalGameData.isPaused = false;
        }

        public void AddRotating(GameObject scythe, int indice) {
            spawnedRotating[indice].spawnedRotating.Add(scythe);

            for (int i = 0; i < spawnedRotating[indice].spawnedRotating.Count; i++) {
                RotateAround rotater = spawnedRotating[indice].spawnedRotating[i].GetComponent<RotateAround>();

                rotater.rotateAroundTransform = player.transform;
                rotater.offset = spawnedRotating[indice].offset;
                rotater.rotationOffset = 360 / spawnedRotating[indice].spawnedRotating.Count * i;
            }
        }

        public void Update()
        {
            float delta = !GlobalGameData.isPaused ? Time.deltaTime : 0;

            if (times == null) return;

            for (int i = 0; i < times.Count; i++) {
                if (times[i].time > 0) {
                    times[i].time -= delta;
                }
                else {
                    times[i].time = times[i].timeOnSet;

                    if (times[i].perkName != null) {
                        PerkWithLevel perk = unlockedPerks.Find((perk) => perk.perk.name == times[i].perkName);
                        if (perk != null) {
                            switch (perk.perk.updateType) {
                                case UpdateType.Regeneration:
                                    player.health.ChangeHealth(perk.level);
                                    break;
                                case UpdateType.DamageZone:
                                    int damage = times[i].perkName switch {
                                        "Electric" => 10,
                                        _ => 0
                                    };
                                    float radius = times[i].perkName switch {
                                        "Electric" => 2.5f + 0.1f * (perk.level - 1),
                                        _ => 0
                                    };

                                    foreach (Collider col in Physics.OverlapSphere(player.transform.position, radius)) {
                                        if (col.TryGetComponent<Enemy>(out var enemy)) {
                                            enemy.DirectDamage(damage);
                                        }
                                    }
                                    break;
                                case UpdateType.Custom:
                                    switch (times[i].perkName) {
                                        case "Chain Lightning":
                                            Instantiate(lightningBolt, player.transform.position, new Quaternion()).GetComponent<ChainDamage>().Init(null, 2 + 1 * (perk.level - 1), 7.5f + 0.5f * (perk.level - 1), -(15 + 5 * (perk.level - 1)));
                                            break;
                                        case "Bone Shield":
                                            if (spawnedRotating[1].spawnedRotating.Count < (2 + perk.level)) {
                                                AddRotating(Instantiate(spawnedRotating[1].prefab, player.transform.position, Quaternion.identity), 1);
                                            }
                                            break;
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class PerkTimerHolder {
        public float time;
        public float timeOnSet;
        public string perkName;
    }

    [System.Serializable]
    public class RotationHolder {
        public List<GameObject> spawnedRotating;
        public GameObject prefab;
        public float offset;
    }

    [System.Serializable]
    public class StringToInt {
        public string perkName;
        public int number;

        public StringToInt(string perkName, int number) {
            this.perkName = perkName;
            this.number = number;
        }
    }
}