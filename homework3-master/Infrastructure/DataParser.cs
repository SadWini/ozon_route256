using Domain;

namespace Infrastructure;

public class DataParser : IDataParser
{
    public ProductRecord StringToProductRecord(string info)
    {
        string[] temp = info.Split(", ");
        if (temp.Length < 3)
            throw new FormatException($"Can't parse item info from line: {info}");
        if (!int.TryParse(temp[0], out int id))
            throw new FormatException($"Id as int expected at line: {info}");
        if (!int.TryParse(temp[1], out int prediction))
            throw new FormatException($"Prediction as int expected at line: {info}");
        if (!int.TryParse(temp[2], out int stock))
            throw new FormatException($"Stock as int expected at line: {info}");

        return new ProductRecord(id, prediction, stock);
    }
}
