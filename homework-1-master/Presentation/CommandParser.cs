using Domain;

namespace Presentation;

public class CommandParser : ICommandParser
{
    public CommandRawParseResult Parse(string input)
    {
        string[] args = input.Trim().ToLower().Split();
        if (args.Length < 2)
            throw new FormatException("Expected command ");
        if (!int.TryParse(args[1], out int id))
            throw new FormatException("You should to specify item ID afted ads");
        var detail = new List<int>();
        detail.Add(id);
        switch (args[0])
        {
            case "ads":
                return new CommandRawParseResult(CommandType.ADS, detail);
            case "prediction":
            case "demand":
                if (!int.TryParse(args[2], out int days))
                    throw new FormatException("You should to specify item ID afted ads");
                detail.Add(days);
                return new CommandRawParseResult(CommandType.ADS, detail);
        }
        throw new FormatException($"Can't parse command from {input}");
    }
}
