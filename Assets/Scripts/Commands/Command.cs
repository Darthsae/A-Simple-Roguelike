using UnityEngine;

namespace ASimpleRoguelike.Commands {
    public class Command {
        public string name;
        public string description;
        public ParameterValue[] parameters;
        public event CommandExecutedHandler function;

        public Command(string name, string description = "", ParameterValue[] parameters = null, CommandExecutedHandler function = null) {
            this.name = name;
            this.description = description;
            this.parameters = parameters;
            this.function = function;
        }

        public bool IsValid(string initializer, string[] parameters) {
            Debug.Log("" + initializer + "" + parameters);
            if (name != initializer) return false;

            Debug.Log("Passed this check");

            if (this.parameters != null) {
                for (int i = 0; i < this.parameters.Length; i++) {
                    if (parameters.Length > i) {
                        switch (this.parameters[i].type) {
                            case ParameterType.INT:
                                Debug.Log("Trying to parse: " + parameters[i] + " as int, it was a " + int.TryParse(parameters[i], out _));
                                if (!int.TryParse(parameters[i], out _)) return false;
                                break;
                            case ParameterType.FLOAT:
                                Debug.Log("Trying to parse: " + parameters[i] + " as float, it was a " + float.TryParse(parameters[i], out _));
                                if (!float.TryParse(parameters[i], out _)) return false;
                                break;
                            case ParameterType.BOOL:
                                Debug.Log("Trying to parse: " + parameters[i] + " as bool, it was a " + bool.TryParse(parameters[i], out _));
                                if (!bool.TryParse(parameters[i], out _)) return false;
                                break;
                            case ParameterType.STRING:
                                break;
                        }
                    }
                    else if (this.parameters[i].required) return false;
                }
            }

            return true;
        }

        public void Execute(CommandHandler commandHandler, string[] parameters) {
            function?.Invoke(commandHandler, parameters);
        }

        public delegate void CommandExecutedHandler(CommandHandler commandHandler, string[] parameters);

        public enum ParameterType {
            INT,
            FLOAT,
            STRING,
            BOOL
        }

        public struct ParameterValue {
            public string name;
            public string description;
            public ParameterType type;
            public bool required;

            public ParameterValue(string name, string description, ParameterType type, bool required = true) {
                this.name = name;
                this.description = description;
                this.type = type;
                this.required = required;
            }
        }
    }
}