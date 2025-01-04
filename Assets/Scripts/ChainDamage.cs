using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ASimpleRoguelike {
    public class ChainDamage : MonoBehaviour
    {
        public int remainingBounces = 3;
        public float maxBounceDistance;

        public ChainDamage parent;

        public GameObject summon;
        public GameObject target;

        public LightningBolt lightningBolt;

        public Owner owner = Owner.Player;

        public void Init(ChainDamage parent, int remainingBounces, float maxBounceDistanceChange, int damage) {
            this.parent = parent;
            this.remainingBounces = remainingBounces;
            maxBounceDistance += maxBounceDistanceChange;

            bool broken = false;

            if (summon != null) {
                List<GameObject> cantHit = parent != null ? parent.GetCantHit(target) : new();
                foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, maxBounceDistance)) {
                    if (broken) break;

                    switch (owner) {
                        case Owner.Player:
                            if (collider.gameObject.TryGetComponent<Entity.Entity>(out var enemy)) {
                                if (cantHit.Contains(collider.gameObject) || Faction.factions[0] == enemy.faction || GlobalGameData.factionRelations[Faction.FactionNamePair(Faction.factions[0], enemy.faction)] > 0) {
                                    continue;
                                }

                                target = collider.gameObject;
                                enemy.DirectDamage(damage, true);

                                if (lightningBolt != null) {
                                    lightningBolt.startPoint = transform.position;
                                    lightningBolt.endPoint = enemy.transform.position;
                                    Debug.Log("Remaining bounces: " + remainingBounces);
                                    if (parent != null) {
                                        //Debug.Log("Parent: " + parent.transform.position);
                                    }
                                    //Debug.Log("Start: " + lightningBolt.startPoint);
                                    //Debug.Log("End: " + lightningBolt.endPoint);
                                    lightningBolt.GenerateLightning();
                                }

                                if (remainingBounces > 0) {
                                    remainingBounces--;
                                    //Debug.Log("Instantiating: " + remainingBounces);
                                    //Debug.Log("Damage: " + damage);
                                    //Debug.Log("This: " + this);
                                    ChainDamage chainDamage = Instantiate(summon, enemy.transform.position, new Quaternion()).GetComponent<ChainDamage>();
                                    //Debug.Log(chainDamage);
                                    chainDamage.Init(this, remainingBounces, 0, damage);
                                }

                                broken = true;
                            }
                            break;
                    }
                }
            }
        }

        public List<GameObject> GetCantHit(GameObject add) {
            if (parent != null) {
                return parent.GetCantHit(target).Concat(new List<GameObject>() { add }).ToList();
            }
            else {
                return new List<GameObject>() { target };
            }
        }
    }
}