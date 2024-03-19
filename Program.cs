using clio;

var message = args?.Length > 0 ? args[0] : string.Empty;

var aggregate = new CommandAggregate();

try
{
    if (!aggregate.Commands.TryGetValue(message, out var command))
        throw new ArgumentException($"The term '{message}' is not a recognized valid command.");

    command.Execute(args);
}
catch (Exception ex)
{
    var color = Console.ForegroundColor;
    Console.ForegroundColor = ConsoleColor.DarkYellow;
    Console.WriteLine(ex.Message);
    Console.ForegroundColor = color;
}