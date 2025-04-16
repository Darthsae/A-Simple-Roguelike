using System;
using System.Collections.Generic;
using System.Linq;
using ASimpleRoguelike.Inventory;
using TMPro;
using UnityEngine;

namespace ASimpleRoguelike.Commands {
    public class CommandHandler : MonoBehaviour {
        public Player player;
        public TimerHandler timerHandler;
        public GameObject[] enemies;
        public TMP_InputField inputField;
        public TMP_Text textField;

        public List<Command> commands = new() {
            new Command("Help", "Lists all commands.", null, 
                (commandHandler, parameters) => {
                    foreach (Command command in commandHandler.commands) {
                        commandHandler.Log($"{command.name}: {command.description}\n");
                    }
                }),
            new Command("SetTime", "Set the game timer.",  new Command.ParameterValue[] {
                    new("Time", "The time to set to.", Commands.Command.ParameterType.INT, true)
                }, (commandHandler, parameters) => {
                    int time = int.Parse(parameters[0]);
                    //Debug.Log(time);
                    commandHandler.timerHandler.time = time;
                }),
            new Command("SetHealth", "Set the player health.",  new Command.ParameterValue[] {
                    new("Health", "The health to set to.", Commands.Command.ParameterType.INT, true)
                }, (commandHandler, parameters) => {
                    int health = int.Parse(parameters[0]);
                    //Debug.Log(health);
                    commandHandler.player.health.SetHealth(health);
                }),
            new Command("SetMaxHealth", "Set the player max health.",  new Command.ParameterValue[] {
                    new("Health", "The max health to set to.", Commands.Command.ParameterType.INT, true)
                }, (commandHandler, parameters) => {
                    int health = int.Parse(parameters[0]);
                    //Debug.Log(health);
                    commandHandler.player.health.SetMaxHealth(health);
                }),
            new Command("ChangeHealth", "Change the player health.",  new Command.ParameterValue[] {
                    new("Health", "The health to change by.", Commands.Command.ParameterType.INT, true)
                }, (commandHandler, parameters) => {
                    int health = int.Parse(parameters[0]);
                    //Debug.Log(health);
                    commandHandler.player.health.ChangeHealth(health);
                }),
            new Command("SetXP", "Set the player XP.",  new Command.ParameterValue[] {
                    new("XP", "The XP to set to.", Commands.Command.ParameterType.INT, true)
                }, (commandHandler, parameters) => {
                    int xp = int.Parse(parameters[0]);
                    //Debug.Log(xp);
                    commandHandler.player.currentXP = xp;
                }),
            new Command("ChangeXP", "Change the player XP.",  new Command.ParameterValue[] {
                    new("XP", "The XP to change by.", Commands.Command.ParameterType.INT, true)
                }, (commandHandler, parameters) => {
                    int xp = int.Parse(parameters[0]);
                    //Debug.Log(xp);
                    commandHandler.player.ChangeXP(xp);
                }),
            new Command("BoomGoBoomABoom", "Spawn a circle of enemies around the player.",  new Command.ParameterValue[] {
                    new("Enemy ID", "The enemy ID to spawn.", Commands.Command.ParameterType.INT, true),
                    new("Distance", "The radius of the circle.", Commands.Command.ParameterType.FLOAT, false)
                }, (commandHandler, parameters) => {
                    int enemy = int.Parse(parameters[0]);
                    if (!float.TryParse(parameters[1], out float distance)) distance = 8f;
                    
                    for (int i = 0; i < 360; i += 15) {
                        GameObject enemyObject = Instantiate(commandHandler.enemies[enemy], commandHandler.player.transform.position + distance * new Vector3(Mathf.Cos(i * Mathf.Deg2Rad), Mathf.Sin(i * Mathf.Deg2Rad)), Quaternion.identity);
                        enemyObject.transform.parent = null;
                    }
                }),
            new Command("ChangeEquipment", "Change equipment.",  new Command.ParameterValue[] {
                    new("Slot ID", "The slot ID to change.", Commands.Command.ParameterType.INT, true),
                    new("Item ID", "The item ID to put in the slot.", Commands.Command.ParameterType.INT, true)
                }, (commandHandler, parameters) => {
                    int slot = int.Parse(parameters[0]);
                    int item = int.Parse(parameters[1]);
                    
                    switch (slot) {
                        case 0: // Head
                            GlobalGameData.headSlot = Item.items[item];
                            break;
                        case 1: // Neck
                            GlobalGameData.neckSlot = Item.items[item];
                            break;
                        case 2: // Chest
                            GlobalGameData.chestSlot = Item.items[item];
                            break;
                        case 3: // Shoulder
                            GlobalGameData.shoulderSlot = Item.items[item];
                            break;
                        case 4: // Upper Arm
                            GlobalGameData.upperArmSlot = Item.items[item];
                            break;
                        case 5: // Elbow
                            GlobalGameData.elbowSlot = Item.items[item];
                            break;
                        case 6: // Forearm
                            GlobalGameData.forearmSlot = Item.items[item];
                            break;
                        case 7: // Hand
                            GlobalGameData.handSlot = Item.items[item];
                            break;
                        case 8: // Finger
                            GlobalGameData.fingerSlot = Item.items[item];
                            break;
                        case 9: // Back
                            GlobalGameData.backSlot = Item.items[item];
                            break;
                        case 10: // Stomach
                            GlobalGameData.stomachSlot = Item.items[item];
                            break;
                        case 11: // Waist
                            GlobalGameData.waistSlot = Item.items[item];
                            break;
                        case 12: // Abdomen
                            GlobalGameData.abdomenSlot = Item.items[item];
                            break;
                        case 13: // Hip
                            GlobalGameData.hipSlot = Item.items[item];
                            break;
                        case 14: // Upper Leg
                            GlobalGameData.upperLegSlot = Item.items[item];
                            break;
                        case 15: // Knee
                            GlobalGameData.kneeSlot = Item.items[item];
                            break;
                        case 16: // Lower Leg
                            GlobalGameData.lowerLegSlot = Item.items[item];
                            break;
                        case 17: // Foot
                            GlobalGameData.footSlot = Item.items[item];
                            break;
                        case 18: // Toe
                            GlobalGameData.toeSlot = Item.items[item];
                            break;
                    }
                }),
            new Command("ClearPause", "Clear all pause reasons.", null, (commandHandler, parameters) => {
                    GlobalGameData.ClearPauseReasons();
                }),
            new Command("LogPauseReasons", "Logs all pause reasons.", null, (commandHandler, parameters) => {
                    foreach (string reason in GlobalGameData.pauseReasons) {
                        Logger.LogInfo(reason);
                    }
                }),
        };

        public void Command(string commandPrompt) {
            //Debug.Log("Command: " + commandPrompt);

            // Parse the arguments
            try {
                string[] mappy = commandPrompt.Split(' ');

                string[] arguments = new string[mappy.Length - 1];

                for (int i = 1; i < mappy.Length; i++) {
                    //Debug.Log($"Argument {i}: {mappy[i]}");
                    arguments[i - 1] = mappy[i];
                }

                foreach (Command command in commands) {
                    if (command.IsValid(mappy[0], arguments)) {
                        //Debug.Log(mappy[0]);
                        command.Execute(this, arguments);
                        break;
                    }
                }
            } catch (Exception e) {
                Debug.Log(e.ToString());
            }
        }

        public void OnChange(string command) {
            if (!string.IsNullOrEmpty(command)) {
                Command(command);
                textField.text += command + "\n";
                inputField.text = "";
            }
        }

        public void Log(string message) {
            textField.text += message;
            textField.rectTransform.sizeDelta.Set(textField.rectTransform.sizeDelta.x, textField.rectTransform.sizeDelta.y + 14 * message.Count(cha => cha == '\n'));
        }
    }
}