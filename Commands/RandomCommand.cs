using System.Windows.Input;

namespace clio.Commands;

internal class RandomCommand : ICommand
{
    private readonly Random _random = new();

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        var args = parameter as string[];

        switch (args[1])
        {
            case "help":
                Console.WriteLine(Help);
                break;
            case "number":
                if (!int.TryParse(args[2], out var min) || !int.TryParse(args[3], out var max))
                {
                    throw new ArgumentException("Invalid arguments");
                }

                Console.WriteLine(Next(min, max));
                break;
            default:
                throw new ArgumentException("Invalid command");
        }
    }

    public int Next(int min, int max) => _random.Next(min, max);

    public int Next(int max) => _random.Next(max);

    public static string Help 
        => "\r\nCommand: random\r\n" +
        "Description: Generate random numbers.\r\n" +
        "\r\nUsage:\r\n" +
        "  random number <min> <max>: Generate a random number between <min> and <max>.\r\n" +
        "Examples:\r\n" +
        "  random number 1 100: Generates a random number between 1 and 100.\r\n" +
        "\r\n";
}