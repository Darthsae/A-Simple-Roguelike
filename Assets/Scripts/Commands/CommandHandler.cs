using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;

namespace ASimpleRoguelike.Commands {
    public class CommandHandler : MonoBehaviour
    {
        public Player player;
        public TimerHandler timerHandler;
        public GameObject[] enemies;
        public TMP_InputField inputField;
        public TMP_Text textField;

        public List<Command> commands = new() {
            new Command("Help", "Lists all commands.", null, 
                (CommandHandler commandHandler, string[] parameters) => {
                    foreach (Command command in commandHandler.commands) {
                        commandHandler.Log($"{command.name}: {command.description}\n");
                    }
                }),
            new Command("SetTime", "Set the game timer.",  new Command.ParameterValue[] {
                    new("Time", "The time to set to.", Commands.Command.ParameterType.INT, true)
                }, (CommandHandler commandHandler, string[] parameters) => {
                    int time = int.Parse(parameters[0]);
                    Debug.Log(time);
                    commandHandler.timerHandler.time = time;
                }),
            new Command("SetHealth", "Set the player health.",  new Command.ParameterValue[] {
                    new("Health", "The health to set to.", Commands.Command.ParameterType.INT, true)
                }, (CommandHandler commandHandler, string[] parameters) => {
                    int health = int.Parse(parameters[0]);
                    Debug.Log(health);
                    commandHandler.player.health.SetHealth(health);
                }),
            new Command("SetMaxHealth", "Set the player max health.",  new Command.ParameterValue[] {
                    new("Health", "The max health to set to.", Commands.Command.ParameterType.INT, true)
                }, (CommandHandler commandHandler, string[] parameters) => {
                    int health = int.Parse(parameters[0]);
                    Debug.Log(health);
                    commandHandler.player.health.SetMaxHealth(health);
                }),
            new Command("ChangeHealth", "Change the player health.",  new Command.ParameterValue[] {
                    new("Health", "The health to change by.", Commands.Command.ParameterType.INT, true)
                }, (CommandHandler commandHandler, string[] parameters) => {
                    int health = int.Parse(parameters[0]);
                    Debug.Log(health);
                    commandHandler.player.health.ChangeHealth(health);
                }),
            new Command("SetXP", "Set the player XP.",  new Command.ParameterValue[] {
                    new("XP", "The XP to set to.", Commands.Command.ParameterType.INT, true)
                }, (CommandHandler commandHandler, string[] parameters) => {
                    int xp = int.Parse(parameters[0]);
                    Debug.Log(xp);
                    commandHandler.player.currentXP = xp;
                }),
            new Command("ChangeXP", "Change the player XP.",  new Command.ParameterValue[] {
                    new("XP", "The XP to change by.", Commands.Command.ParameterType.INT, true)
                }, (CommandHandler commandHandler, string[] parameters) => {
                    int xp = int.Parse(parameters[0]);
                    Debug.Log(xp);
                    commandHandler.player.ChangeXP(xp);
                }),
            new Command("BoomGoBoomABoom", "Spawn a circle of enemies around the player.",  new Command.ParameterValue[] {
                    new("Enemy ID", "The enemy ID to spawn.", Commands.Command.ParameterType.INT, true),
                    new("Distance", "The radius of the circle.", Commands.Command.ParameterType.FLOAT, false)
                }, (CommandHandler commandHandler, string[] parameters) => {
                    int enemy = int.Parse(parameters[0]);
                    if (!float.TryParse(parameters[1], out float distance)) distance = 8f;
                    
                    for (int i = 0; i < 360; i += 15) {
                        GameObject enemyObject = Instantiate(commandHandler.enemies[enemy], commandHandler.player.transform.position + distance * new Vector3(Mathf.Cos(i * Mathf.Deg2Rad), Mathf.Sin(i * Mathf.Deg2Rad)), Quaternion.identity);
                        enemyObject.transform.parent = null;
                    }
                }),
        };

        public void Command(string commandPrompt) {
            Debug.Log("Command: " + commandPrompt);

            // Parse the arguments
            try {
                string[] mappy = commandPrompt.Split(' ');

                string[] arguments = commandPrompt[mappy[0].Length..].Split(' ');

                for (int i = 0; i < mappy.Length; i++) {
                    Debug.Log($"Argument {i}: {mappy[i]}");
                }

                foreach (Command command in commands) {
                    if (command.IsValid(mappy[0], arguments)) {
                        Debug.Log(mappy[0]);
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
            textField.rectTransform.sizeDelta.Set(textField.rectTransform.sizeDelta.x, textField.rectTransform.sizeDelta.y + 14 * message.Count((char cha) => cha == '\n'));
        }
    }
}