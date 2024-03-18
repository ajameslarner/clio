using clio.Core;

namespace clio;

internal class CommandAggregate : CommandPalette
{
    public CommandAggregate()
    {
        AddCommand("random", new Commands.RandomCommand());
    }
}