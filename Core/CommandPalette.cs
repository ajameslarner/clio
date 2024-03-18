using System.Windows.Input;

namespace clio.Core;

internal class CommandPalette
{
    public readonly IDictionary<string, ICommand> Commands = new Dictionary<string, ICommand>();

    public void AddCommand(string command, ICommand context)
    {
        Commands.Add(command, context);
    }
}