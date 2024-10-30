namespace Domain;

public interface ICommandParser
{
    public CommandRawParseResult Parse(string input);
}
