using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ASimpleRoguelike.Inventory;
using ASimpleRoguelike.Equinox;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;

namespace ASimpleRoguelike {
    public class GlobalGameData : MonoBehaviour {
        public static ItemData item;
        public static Image spriteRenderer;

        public static bool moveMode = true;

        public static TMP_Text tooltipText;
        public static GameObject tooltip;

        public Sprite[] barrelSprites;
        public GameObject pickup;
        public Player player;
        public TimerHandler timer;

        public static List<string> pauseReasons = new();

        #region Items
        public static ItemData headSlot;
        public static ItemData neckSlot;
        public static ItemData chestSlot;
        public static ItemData shoulderSlot;
        public static ItemData upperArmSlot;
        public static ItemData elbowSlot;
        public static ItemData forearmSlot;
        public static ItemData handSlot;
        public static ItemData fingerSlot;
        public static ItemData backSlot;
        public static ItemData stomachSlot;
        public static ItemData waistSlot;
        public static ItemData abdomenSlot;
        public static ItemData hipSlot;
        public static ItemData upperLegSlot;
        public static ItemData kneeSlot;
        public static ItemData lowerLegSlot;
        public static ItemData footSlot;
        public static ItemData toeSlot;
        #endregion

        public static GameContext gameContext = GameContext.Default;

        public static bool isPaused = false;

        public static double longestTime = 0;
        public static int highestPhase = 0;

        #region Settings
        public static AudioMixer audioMixer;
        public static List<string> audioMixerGroups = new();
        public static float[] audioMixerVolumes;
        #endregion

        #region Flags for Major Unlocks
        public static bool unlockedEquinox;
        public static bool[] unlockedEquinoxes;
        public static bool unlockedItem;
        public static bool[] unlockedItems;
        #endregion

        #region Factions
        public static Dictionary<string, int> factionRelations = new();
        #endregion

        #region Pause Handling
        public static void ClearPauseReasons() {
            pauseReasons.Clear();
            isPaused = false;
        }

        public static void AddPauseReason(string reason) {
            if (!isPaused) {
                isPaused = true;
            }

            pauseReasons.Add(reason);
        }

        public static void RemovePauseReason(string reason) {
            pauseReasons.Remove(reason);

            if (pauseReasons.Count == 0) {
                isPaused = false;
            }
        }
        #endregion

        public static void NewData() {
            #region Equinoxes
            unlockedEquinox = false;
            unlockedEquinoxes = new bool[Equinox.Equinox.EquinoxCount];

            for (int i = 0; i < unlockedEquinoxes.Length; i++) {
                unlockedEquinoxes[i] = false;
            }
            #endregion

            #region Items
            unlockedItem = true;
            unlockedItems = new bool[Item.ItemCount];

            for (int i = 0; i < unlockedItems.Length; i++) {
                unlockedItems[i] = true;
                //Debug.Log($"Unlocked item: {Item.items[i].name}");
            }
            #endregion

            #region Factions
            for (int i = 0; i < Faction.FactionCount - 1; i++) {
                for (int j = i + 1; j < Faction.FactionCount; j++) {
                    Debug.Log("Setting relation between " + Faction.factions[i] + " and " + Faction.factions[j] + " to 0");
                    factionRelations[Faction.FactionNamePair(Faction.factions[i], Faction.factions[j])] = 0;
                }
            }
            #endregion
        
            #region Settings
            audioMixerVolumes = new float[audioMixerGroups.Count]; // Set all volumes to 1ss
            for (int i = 0; i < audioMixerGroups.Count; i++) {
                audioMixerVolumes[i] = 0;
            }
            #endregion

            #region Equips

            #endregion
        }

        public static void LoadData() {
            #region Equinoxes
            for (int i = 0; i < unlockedEquinoxes.Length; i++) {
                unlockedEquinoxes[i] = false;
            }
            #endregion

            #region Items
            for (int i = 0; i < unlockedItems.Length; i++) {
                unlockedItems[i] = false;
                //Debug.Log($"Locked item: {Item.items[i].name}");
            }
            #endregion

            #region Factions
            #endregion

            #region Settings

            #endregion

            #region Equips
            
            #endregion

            try {
                FileDataHandler.LoadData();
            }
            catch (Exception e) {
                Debug.LogError("Error loading data: " + e.Message);
                NewData();
            }
        }

        public static void SaveData() {
            try {
                FileDataHandler.SaveData();
            }
            catch (Exception e) {
                Debug.LogError("Error saving data: " + e.Message);
            }
        }

        /// <summary>
        /// Sets the item in the player's inventory. If the item is not null and has a sprite, the sprite is displayed.
        /// </summary>
        /// <param name="newItem">The new item to set.</param>
        public static void SetItem(ItemData newItem) {
            item = newItem;
            if (item != null && item.sprite != null) {
                spriteRenderer.sprite = item.sprite;
                spriteRenderer.gameObject.SetActive(true);
            } else {
                spriteRenderer.gameObject.SetActive(false);
            }
        }
    }

    public static class FileDataHandler {
        private static readonly string dataDirPath = Path.Combine(Application.persistentDataPath, "Saves");
        private static readonly string dataFileName = "data.sav";  

        public static void LoadData() { 
            string fullPath = Path.Combine(dataDirPath, dataFileName);
            try {
                if (File.Exists(fullPath)) {
                    Debug.Log("Loading data from " + fullPath);
                    using FileStream stream = new(fullPath, FileMode.Open);
                    using BinaryReader reader = new(stream);

                    #region Equinoxes
                    GlobalGameData.unlockedEquinox = reader.ReadBoolean(); // Equinox Unlocked flag
                    reader.ReadArray(ref GlobalGameData.unlockedEquinoxes, Equinox.Equinox.EquinoxCount); // Unlocked Equinoxes
                    #endregion

                    #region Items
                    GlobalGameData.unlockedItem = reader.ReadBoolean(); // Item Unlocked flag
                    reader.ReadArray(ref GlobalGameData.unlockedItems, Item.ItemCount); // Unlocked Items
                    #endregion

                    GlobalGameData.longestTime = reader.ReadDouble(); // Longest Time
                    GlobalGameData.highestPhase = reader.ReadInt32(); // Highest Phase
                    GlobalGameData.gameContext = (GameContext)reader.ReadInt32(); // Game Context

                    #region Factions
                    long longer = reader.ReadInt64();
                    for (int i = 0; i < longer; i++) {
                        GlobalGameData.factionRelations[reader.ReadString()] = reader.ReadInt32();
                    }
                    #endregion

                    #region Settings
                    reader.ReadArray(ref GlobalGameData.audioMixerVolumes, GlobalGameData.audioMixerGroups.Count);
                    for (int i = 0; i < GlobalGameData.audioMixerVolumes.Length; i++) {
                        if (GlobalGameData.audioMixerVolumes[i] == 0)
                        {
                            GlobalGameData.audioMixer.SetFloat(GlobalGameData.audioMixerGroups[i], -100);
                            return;
                        }
                        
                        GlobalGameData.audioMixer.SetFloat(GlobalGameData.audioMixerGroups[i], Mathf.Log10(GlobalGameData.audioMixerVolumes[i]) * 20);
                    }
                    #endregion

                    #region Equips
                    /* SLOTS
                    headSlot
                    neckSlot
                    chestSlot
                    shoulderSlot
                    upperArmSlot
                    elbowSlot
                    forearmSlot
                    handSlot
                    fingerSlot
                    backSlot
                    stomachSlot
                    waistSlot
                    abdomenSlot
                    hipSlot
                    upperLegSlot
                    kneeSlot
                    lowerLegSlot
                    footSlot
                    toeSlot*/

                    GlobalGameData.headSlot = Item.items[reader.ReadInt32()];
                    GlobalGameData.neckSlot = Item.items[reader.ReadInt32()];
                    GlobalGameData.chestSlot = Item.items[reader.ReadInt32()];
                    GlobalGameData.shoulderSlot = Item.items[reader.ReadInt32()];
                    GlobalGameData.upperArmSlot = Item.items[reader.ReadInt32()];
                    GlobalGameData.elbowSlot = Item.items[reader.ReadInt32()];
                    GlobalGameData.forearmSlot = Item.items[reader.ReadInt32()];
                    GlobalGameData.handSlot = Item.items[reader.ReadInt32()];
                    GlobalGameData.fingerSlot = Item.items[reader.ReadInt32()];
                    GlobalGameData.backSlot = Item.items[reader.ReadInt32()];
                    GlobalGameData.stomachSlot = Item.items[reader.ReadInt32()];
                    GlobalGameData.waistSlot = Item.items[reader.ReadInt32()];
                    GlobalGameData.abdomenSlot = Item.items[reader.ReadInt32()];
                    GlobalGameData.hipSlot = Item.items[reader.ReadInt32()];
                    GlobalGameData.upperLegSlot = Item.items[reader.ReadInt32()];
                    GlobalGameData.kneeSlot = Item.items[reader.ReadInt32()];
                    GlobalGameData.lowerLegSlot = Item.items[reader.ReadInt32()];
                    GlobalGameData.footSlot = Item.items[reader.ReadInt32()];
                    GlobalGameData.toeSlot = Item.items[reader.ReadInt32()];
                    EquinoxHandler.currentEquinox = reader.ReadInt32();
                    #endregion
                }
            }
            catch (Exception e) {
                Debug.LogError("Error reading data directory: " + e.Message);
            }
        }

        public static void SaveData() {
            string fullPath = Path.Combine(dataDirPath, dataFileName);
            try {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                using FileStream stream = new(fullPath, FileMode.Create);
                using BinaryWriter writer = new(stream);

                #region Equinoxes
                writer.Write(GlobalGameData.unlockedEquinox); // Equinox Unlocked flag
                writer.WriteArray(GlobalGameData.unlockedEquinoxes); // Unlocked Equinoxes
                #endregion                

                #region Items
                writer.Write(GlobalGameData.unlockedItem); // Item Unlocked flag
                writer.WriteArray(GlobalGameData.unlockedItems); // Unlocked Items
                #endregion

                writer.Write(GlobalGameData.longestTime); // Longest Time
                writer.Write(GlobalGameData.highestPhase); // Highest Phase
                writer.Write((int)GlobalGameData.gameContext); // Game Context

                #region Factions
                writer.Write(GlobalGameData.factionRelations.Count);
                foreach (string key in GlobalGameData.factionRelations.Keys) {
                    writer.Write(key);
                    writer.Write(GlobalGameData.factionRelations[key]);
                }
                #endregion

                #region Settings
                writer.WriteArray(GlobalGameData.audioMixerVolumes);
                #endregion

                #region Equips
                /* SLOTS
                headSlot
                neckSlot
                chestSlot
                shoulderSlot
                upperArmSlot
                elbowSlot
                forearmSlot
                handSlot
                fingerSlot
                backSlot
                stomachSlot
                waistSlot
                abdomenSlot
                hipSlot
                upperLegSlot
                kneeSlot
                lowerLegSlot
                footSlot
                toeSlot*/

                writer.Write(Item.items.FindIndex(item => item == GlobalGameData.headSlot));
                writer.Write(Item.items.FindIndex(item => item == GlobalGameData.neckSlot));
                writer.Write(Item.items.FindIndex(item => item == GlobalGameData.chestSlot));
                writer.Write(Item.items.FindIndex(item => item == GlobalGameData.shoulderSlot));
                writer.Write(Item.items.FindIndex(item => item == GlobalGameData.upperArmSlot));
                writer.Write(Item.items.FindIndex(item => item == GlobalGameData.elbowSlot));
                writer.Write(Item.items.FindIndex(item => item == GlobalGameData.forearmSlot));
                writer.Write(Item.items.FindIndex(item => item == GlobalGameData.handSlot));
                writer.Write(Item.items.FindIndex(item => item == GlobalGameData.fingerSlot));
                writer.Write(Item.items.FindIndex(item => item == GlobalGameData.backSlot));
                writer.Write(Item.items.FindIndex(item => item == GlobalGameData.stomachSlot));
                writer.Write(Item.items.FindIndex(item => item == GlobalGameData.waistSlot));
                writer.Write(Item.items.FindIndex(item => item == GlobalGameData.abdomenSlot));
                writer.Write(Item.items.FindIndex(item => item == GlobalGameData.hipSlot));
                writer.Write(Item.items.FindIndex(item => item == GlobalGameData.upperLegSlot));
                writer.Write(Item.items.FindIndex(item => item == GlobalGameData.kneeSlot));
                writer.Write(Item.items.FindIndex(item => item == GlobalGameData.lowerLegSlot));
                writer.Write(Item.items.FindIndex(item => item == GlobalGameData.footSlot));
                writer.Write(Item.items.FindIndex(item => item == GlobalGameData.toeSlot));

                writer.Write(EquinoxHandler.currentEquinox);
                #endregion
            }
            catch (Exception e) {
                Debug.LogError("Error creating data directory: " + e.Message);
            }
        }
    }

    public static class BinaryHandlerExtensions {
        /// <summary>
        /// Writes an array of booleans to the current stream.
        /// </summary>
        /// <param name="writer">The binary writer to write to.</param>
        /// <param name="array">The array of booleans to write to the stream.</param>
        public static void WriteArray(this BinaryWriter writer, bool[] array) {
            writer.Write(array.Length);
            for (int i = 0; i < array.Length; i++) {
                writer.Write(array[i]);
            }
        }

        /// <summary>
        /// Reads an array of booleans from the current stream.
        /// </summary>
        /// <param name="reader">The binary reader to read from.</param>
        /// <param name="array">The array to read into.</param>
        public static void ReadArray(this BinaryReader reader, ref bool[] array, int lengthMin = 0) {
            int length = reader.ReadInt32();
            array = new bool[length < lengthMin ? lengthMin : length];
            for (int i = 0; i < length; i++) {
                array[i] = reader.ReadBoolean();
            }
        }

        /// <summary>
        /// Writes the length of the provided array and then writes each element of the array to the binary stream.
        /// </summary>
        /// <param name="writer">The BinaryWriter instance to write the data to.</param>
        /// <param name="array">The array of integers to write to the stream.</param>
        public static void WriteArray(this BinaryWriter writer, int[] array) {
            writer.Write(array.Length);
            for (int i = 0; i < array.Length; i++) {
                writer.Write(array[i]);
            }
        }

        /// <summary>
        /// Reads an array of integers from the binary stream and initializes the provided array reference with the read values.
        /// </summary>
        /// <param name="reader">The BinaryReader instance to read the data from.</param>
        /// <param name="array">The reference to the integer array to populate with data from the stream.</param>
        public static void ReadArray(this BinaryReader reader, ref int[] array, int lengthMin = 0) {
            int length = reader.ReadInt32();
            array = new int[length < lengthMin ? lengthMin : length];
            for (int i = 0; i < length; i++) {
                array[i] = reader.ReadInt32();
            }
        }

        public static void WriteArray(this BinaryWriter writer, float[] array) {
            writer.Write(array.Length);
            for (int i = 0; i < array.Length; i++) {
                writer.Write(array[i]);
            }
        }

        public static void ReadArray(this BinaryReader reader, ref float[] array, int lengthMin = 0) {
            int length = reader.ReadInt32();
            array = new float[length < lengthMin ? lengthMin : length];
            for (int i = 0; i < length; i++) {
                array[i] = reader.ReadSingle();
            }
        }
    }
}