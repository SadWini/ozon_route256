namespace Domain;

public class SaleRecord
{
    public int Id { get; init; }
    public DateTime Date { get; init; }
    public int Sales { get; init; }
    public int Stock { get; init; }

    public SaleRecord(int id, DateTime date, int sales, int stock)
    {
        Id = id;
        Date = date;
        Sales = sales;
        Stock = stock;
    }
}
