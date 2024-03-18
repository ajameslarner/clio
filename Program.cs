using clio;

var message = args?.Length > 0 ? args[0] : null;

if (message is null)
{
    Console.WriteLine("No command provided");
    return;
}

var aggregate = new CommandAggregate();

try
{
    var command = aggregate.Commands[message];
    command.Execute(args);
}
catch (ArgumentException e)
{
    Console.WriteLine(e.Message);
}