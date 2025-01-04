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
            if (name != initializer) return false;

            if (this.parameters != null) {
                for (int i = 0; i < this.parameters.Length; i++) {
                    if (parameters.Length > i) {
                        switch (this.parameters[i].type) {
                            case ParameterType.INT:
                                if (!int.TryParse(parameters[i], out _)) return false;
                                break;
                            case ParameterType.FLOAT:
                                if (!float.TryParse(parameters[i], out _)) return false;
                                break;
                            case ParameterType.BOOL:
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