using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ASimpleRoguelike.Entity;

namespace ASimpleRoguelike {
    public class Spawner : MonoBehaviour
    {
        public TimerHandler phaseManager;
        public PerkManager perkManager;

        public List<SpawnHolder> spawnables = new();
        public float TotalWeight => spawnables.FindAll(x => x.minimumPhase <= phaseManager.GetPhase() && x.maximumPhase >= phaseManager.GetPhase() && x.minimumGodState <= PerkManager.godsAttention && x.maximumGodState >= PerkManager.godsAttention).Sum(x => x.Weight);
        public Vector2 spawnRange = new(5f, 5f);
        public float spawnRate = 1f;
        private float timeSinceLastSpawn = 0f;
        public int maxSpawn = 10;
        public int spawnedCount = 0;

        public bool spawning = true;

        public GameObject GetSpawnable() {
            float random = UnityEngine.Random.Range(0f, TotalWeight);
            
            foreach (var spawnable in spawnables) {
                if (spawnable.minimumPhase > phaseManager.GetPhase() || spawnable.maximumPhase < phaseManager.GetPhase() || spawnable.minimumGodState > PerkManager.godsAttention || spawnable.maximumGodState < PerkManager.godsAttention) 
                    continue;
                
                if (random < spawnable.Weight) {
                    return spawnable.ToSpawn;
                } else {
                    random -= spawnable.Weight;
                }
            }

            return null;
        }

        public static Vector2 InArea(Vector2 range) {
            return new Vector2(UnityEngine.Random.Range(-range.x, range.x), UnityEngine.Random.Range(-range.y, range.y));
        }

        private void Update() {
            if (GlobalGameData.isPaused || !spawning) return;
            
            if (timeSinceLastSpawn < 0f) {
                GameObject spawned = Instantiate(GetSpawnable(), transform.position + (Vector3)InArea(spawnRange), Quaternion.identity);
                spawned.transform.parent = null;
                spawned.transform.position = transform.position + (Vector3)InArea(spawnRange);
                timeSinceLastSpawn = spawnRate;
                spawnedCount++;
                Enemy enemyScript = spawned.GetComponent<Enemy>();
                enemyScript.spawner = this;

                if (perkManager.unlockedPerks.FindIndex((park) => park.perk.name == "Fateweavers Gamble") != -1) {
                    enemyScript.health.SetMaxHealth((int)(enemyScript.health.maxHealth * 1.1f));
                    enemyScript.speed *= 1.1f;
                    enemyScript.attackDamage *= 1.1f;
                    enemyScript.attackRange *= 1.1f;
                    enemyScript.attackRate *= 0.9f;
                }

                enemyScript.health.OnHealthZero += () => {
                    foreach (string perkName in perkManager.onKills) {
                        PerkWithLevel perk = perkManager.unlockedPerks.Find((perk) => perk.perk.name == perkName);
                        switch (perk.perk.onKill) {
                            case KillType.LifeSteal:
                                switch (perkName) {
                                    case "Berserkers Rage":
                                        perkManager.player.health.ChangeHealth((int)(perk.level * 0.01f * perkManager.player.health.maxHealth));
                                        break;
                                    default:
                                        perkManager.player.health.ChangeHealth(perk.level);
                                        break;
                                }
                                break;
                            case KillType.Custom:
                                switch (perkName) {
                                    case "Grave Pact":
                                        if (UnityEngine.Random.Range(0, 100) < 10 + 5 * (perk.level - 1)) {
                                            GameObject thrall = Instantiate(perkManager.graveThrall, enemyScript.transform.position, enemyScript.transform.rotation);
                                            PlayerMinion minion = thrall.GetComponent<PlayerMinion>();
                                            minion.health.SetMaxHealth((int)(enemyScript.health.maxHealth * 0.5f));
                                            minion.attackDamage = enemyScript.attackDamage * 0.75f;
                                            minion.attackRate = enemyScript.attackRate * 1.1f;
                                            minion.speed = enemyScript.speed * 0.95f;
                                            thrall.transform.localScale = enemyScript.transform.localScale;
                                            DestroyAfter destroyAfter = thrall.GetComponent<DestroyAfter>();
                                            destroyAfter.time = 10 + 3 * (perk.level - 1);
                                            minion.health.OnHealthZero += () => { 
                                                foreach (Collider collider in Physics.OverlapSphere(thrall.transform.position, 5f)) {
                                                    if (collider.gameObject.TryGetComponent<Enemy>(out var enemy)) {
                                                        enemy.DirectDamage(-10);
                                                    }
                                                }
                                            };
                                        }
                                        break;
                                }
                                break;
                        }
                    }
                };
            }
            else {
                timeSinceLastSpawn -= Time.deltaTime;
            }
        }
    }

    [Serializable]
    public class SpawnHolder
    {
        public GameObject ToSpawn;
        public float Weight;
        public int minimumPhase = 0;
        public int maximumPhase = int.MaxValue;
        public GodState minimumGodState = GodState.NONE;
        public GodState maximumGodState = GodState.ALL;
    }
}