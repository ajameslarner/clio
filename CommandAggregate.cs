using clio.Commands;
using clio.Core;

namespace clio;

internal class CommandAggregate : CommandPalette
{
    public CommandAggregate()
    {
        AddCommand("belt", new BeltCommand());
    }
}