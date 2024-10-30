using Domain;

namespace Presentation;
public class DataParser : IDataParser
{
    private static string _columns = "ID_DATE_SALES_STOCK";

    public SaleRecord StringToSaleRecord(string info, int line)
    {
        if (line == 0)
        {
            bool isRightColumns = IsValidColumnNames(info);
            if (!isRightColumns)
                throw new FormatException(
                    "Didn't find columns id date sales stock at line 0");
        }

        string[] temp = info.Trim().Split(", ");
        if (temp.Length != 4)
            throw new FormatException($"There are must be exactly four values at {line} line");

        if (!int.TryParse(temp[0], out int id))
            throw new FormatException($"Id as int expected at {line} line");
        if (!DateTime.TryParse(temp[1], out DateTime date))
            throw new FormatException($"There is no date at {line} line");
        if (!int.TryParse(temp[2], out int sales))
            throw new FormatException($"Sales as int expected at {line} line");
        if (!int.TryParse(temp[3], out int stock))
            throw new FormatException($"Stock as int expected at {line} line");

        return new SaleRecord(id, date, sales, stock);
    }

    private static bool IsValidColumnNames(string names)
    {
        string temp = String.Join("_", names
                .Split(", ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            .ToUpper();
        return temp.Equals(_columns);
    }
}
