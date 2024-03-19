using System.Text.Json;
using System.Windows.Input;
using System.Diagnostics;

namespace clio.Commands
{
    internal class BeltCommand : ICommand
    {
        private Dictionary<string, string> _commands = new Dictionary<string, string>();
        private readonly string _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "clio_commands.json");

        public event EventHandler? CanExecuteChanged;

        public BeltCommand()
        {
            LoadCommands();
        }

        public bool CanExecute(object? parameter)
        {
            if (parameter is not string[] args || args.Length < 2)
                return false;

            return _commands.ContainsKey(args[1]);
        }

        public void Execute(object? parameter)
        {
            var args = parameter as string[];
            ArgumentNullException.ThrowIfNull(args);

            if (args.Length < 2)
                throw new ArgumentException("Invalid command format. Expected format is 'clio belt <arg> <arg>'.");

            switch (args[1])
            {
                case "/help":
                    HelpCommand();
                    break;
                case "-a":
                case "-add":
                    string command = string.Join(" ", args.Skip(2));
                    AddCommand(command);
                    break;
                case "-d":
                case "-display":
                    DisplayCommand();
                    break;
                case "-e":
                case "-execute":
                case "-r":
                    ExecuteCommand(args[2]);
                    break;
                default:
                    throw new ArgumentException("Invalid command type. Use /help for a list of commands.");
            }
        }

        public void AddCommand(string command)
        {
            var splitCommand = command.Split(['='], 2);

            if (splitCommand.Length != 2)
                throw new ArgumentException("Invalid command format. Expected format is 'name=command'.");

            _commands[splitCommand[0].Trim()] = splitCommand[1].Trim();
            SaveCommands();
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Command '{splitCommand[0]}' added.");
            Console.ForegroundColor = color;
        }

        public void DisplayCommand()
        {
            if (_commands.Count != 0)
            {
                int maxKeyLength = _commands.Max(command => command.Key.Length);
                Console.WriteLine(Environment.NewLine);
                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"{"Command".PadRight(maxKeyLength)} : {"Description"}");
                Console.WriteLine(new string('-', maxKeyLength + 15));

                foreach (var command in _commands)
                {
                    Console.WriteLine($"{command.Key.PadRight(maxKeyLength)} : {command.Value}");
                }
                Console.WriteLine(Environment.NewLine);
                Console.ForegroundColor = color;
            }
        }

        public void ExecuteCommand(string commandName)
        {
            if (!_commands.TryGetValue(commandName, out var command))
                throw new ArgumentException($"Command '{commandName}' not found.");

            var processStartInfo = new ProcessStartInfo
            {
                FileName = "powershell",
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = false,
                Arguments = "-NoLogo "
            };

            var process = new Process
            {
                StartInfo = processStartInfo
            };

            process.Start();

            using (StreamWriter sw = process.StandardInput)
            {
                if (sw.BaseStream.CanWrite)
                {
                    sw.WriteLine(command);
                }
            }

            process.WaitForExit();
        }

        private void SaveCommands()
        {
            var json = JsonSerializer.Serialize(_commands);
            File.WriteAllText(_filePath, json);
        }

        private static void HelpCommand()
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  ____ _ _       ");
            Console.WriteLine(" / ___| (_)_____ ");
            Console.WriteLine("| |   | | |  _  | ");
            Console.WriteLine("| |___| | | |_| |");
            Console.WriteLine(" \\____|_|_|_____|");
            Console.WriteLine();
            Console.WriteLine("Belt Command Help:");
            Console.WriteLine("-------------------");
            Console.WriteLine("/help                        \t\t: Displays this help text.");
            Console.WriteLine("-add (-a) [name]=[command]   \t\t: Stores a new command with the given name and command text.");
            Console.WriteLine("-display (-d)                \t\t: Displays all stored commands.");
            Console.WriteLine("-execute (-e) (-r) [name]    \t\t: Executes the command with the given name.");
            Console.WriteLine();
            Console.WriteLine("Example usage:");
            Console.WriteLine("clio belt -add greet=echo Hello!");
            Console.WriteLine("clio belt -display");
            Console.WriteLine("clio belt -r greet");
            Console.WriteLine();
            Console.ForegroundColor = color;
        }

        private void LoadCommands()
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);

                if (string.IsNullOrEmpty(json))
                    throw new ArgumentException("Failed to load commands, invalid JSON format.");

                _commands = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? throw new ArgumentException("Failed to load commands, invalid JSON format.");
            }
        }
    }
}