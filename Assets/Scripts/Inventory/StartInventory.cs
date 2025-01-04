using UnityEngine;

namespace ASimpleRoguelike.Inventory {
    public class StartInventory : MonoBehaviour {
        public Inventory inventory;
        public Slot headSlot;
        public Slot neckSlot;
        public Slot chestSlot;
        public Slot shoulderSlot;
        public Slot upperArmSlot;
        public Slot elbowSlot;
        public Slot forearmSlot;
        public Slot handSlot;
        public Slot fingerSlot;
        public Slot backSlot;
        public Slot stomachSlot;
        public Slot waistSlot;
        public Slot abdomenSlot;
        public Slot hipSlot;
        public Slot upperLegSlot;
        public Slot kneeSlot;
        public Slot lowerLegSlot;
        public Slot footSlot;
        public Slot toeSlot;

        void Start() {
            inventory.Init();

            GlobalGameData.SetItem(null);

            Debug.Log("Unlocked items length: " + Item.ItemCount);
            Debug.Log(GlobalGameData.unlockedItems.Length);

            for (int i = 0; i < GlobalGameData.unlockedItems.Length; i++) {
                Debug.Log("Unlocked item: " + GlobalGameData.unlockedItems[i]);
                if (GlobalGameData.unlockedItems[i]) {
                    Debug.Log($"Adding {Item.items[i].name} to inventory");
                    // Log the tags
                    for (int j = 0; j < Item.items[i].tags.Length; j++) {
                        Debug.Log($"Tag {j}: {Item.items[i].tags[j]}");
                    }
                    inventory.QuickAdd(Item.items[i], false);

                    foreach (string tag in Item.items[i].tags) {
                        Debug.Log("Tag: " + tag);
                        switch (tag) {
                            case "head":
                                headSlot.gameObject.SetActive(true);
                                break;
                            case "neck":
                                neckSlot.gameObject.SetActive(true);
                                break;
                            case "chest":
                                chestSlot.gameObject.SetActive(true);
                                break;
                            case "shoulder":
                                shoulderSlot.gameObject.SetActive(true);
                                break;
                            case "upperarm":
                                upperArmSlot.gameObject.SetActive(true);
                                break;
                            case "elbow":
                                elbowSlot.gameObject.SetActive(true);
                                break;
                            case "forearm":
                                forearmSlot.gameObject.SetActive(true);
                                break;
                            case "hand":
                                handSlot.gameObject.SetActive(true);
                                break;
                            case "finger":
                                fingerSlot.gameObject.SetActive(true);
                                break;
                            case "back":
                                backSlot.gameObject.SetActive(true);
                                break;
                            case "stomach":
                                stomachSlot.gameObject.SetActive(true);
                                break;
                            case "waist":
                                waistSlot.gameObject.SetActive(true);
                                break;
                            case "abdomen":
                                abdomenSlot.gameObject.SetActive(true);
                                break;
                            case "hip":
                                hipSlot.gameObject.SetActive(true);
                                break;
                            case "upperleg":
                                upperLegSlot.gameObject.SetActive(true);
                                break;
                            case "knee":
                                kneeSlot.gameObject.SetActive(true);
                                break;
                            case "lowerleg":
                                lowerLegSlot.gameObject.SetActive(true);
                                break;
                            case "foot":
                                footSlot.gameObject.SetActive(true);
                                break;
                            case "toe":
                                toeSlot.gameObject.SetActive(true);
                                break;
                            default:
                                Debug.LogWarning($"Unexpected tag: {tag}");
                                break;
                        }
                    }
                }
            }
        
            headSlot.OnItemChanged += item => GlobalGameData.headSlot = item;
            neckSlot.OnItemChanged += item => GlobalGameData.neckSlot = item;
            chestSlot.OnItemChanged += item => GlobalGameData.chestSlot = item;
            shoulderSlot.OnItemChanged += item => GlobalGameData.shoulderSlot = item;
            upperArmSlot.OnItemChanged += item => GlobalGameData.upperArmSlot = item;
            elbowSlot.OnItemChanged += item => GlobalGameData.elbowSlot = item;
            forearmSlot.OnItemChanged += item => GlobalGameData.forearmSlot = item;
            handSlot.OnItemChanged += item => GlobalGameData.handSlot = item;
            fingerSlot.OnItemChanged += item => GlobalGameData.fingerSlot = item;
            backSlot.OnItemChanged += item => GlobalGameData.backSlot = item;
            stomachSlot.OnItemChanged += item => GlobalGameData.stomachSlot = item;
            waistSlot.OnItemChanged += item => GlobalGameData.waistSlot = item;
            abdomenSlot.OnItemChanged += item => GlobalGameData.abdomenSlot = item;
            hipSlot.OnItemChanged += item => GlobalGameData.hipSlot = item;
            upperLegSlot.OnItemChanged += item => GlobalGameData.upperLegSlot = item;
            kneeSlot.OnItemChanged += item => GlobalGameData.kneeSlot = item;
            lowerLegSlot.OnItemChanged += item => GlobalGameData.lowerLegSlot = item;
            footSlot.OnItemChanged += item => GlobalGameData.footSlot = item;
            toeSlot.OnItemChanged += item => GlobalGameData.toeSlot = item;

            headSlot.SetItem(GlobalGameData.headSlot);
            neckSlot.SetItem(GlobalGameData.neckSlot);
            chestSlot.SetItem(GlobalGameData.chestSlot);
            shoulderSlot.SetItem(GlobalGameData.shoulderSlot);
            upperArmSlot.SetItem(GlobalGameData.upperArmSlot);
            elbowSlot.SetItem(GlobalGameData.elbowSlot);
            forearmSlot.SetItem(GlobalGameData.forearmSlot);
            handSlot.SetItem(GlobalGameData.handSlot);
            fingerSlot.SetItem(GlobalGameData.fingerSlot);
            backSlot.SetItem(GlobalGameData.backSlot);
            stomachSlot.SetItem(GlobalGameData.stomachSlot);
            waistSlot.SetItem(GlobalGameData.waistSlot);
            abdomenSlot.SetItem(GlobalGameData.abdomenSlot);
            hipSlot.SetItem(GlobalGameData.hipSlot);
            upperLegSlot.SetItem(GlobalGameData.upperLegSlot);
            kneeSlot.SetItem(GlobalGameData.kneeSlot);
            lowerLegSlot.SetItem(GlobalGameData.lowerLegSlot);
            footSlot.SetItem(GlobalGameData.footSlot);
            toeSlot.SetItem(GlobalGameData.toeSlot);
        }
    }
}