namespace Domain;

public class ProductRecord
{
    public int Id { get; init; }
    public int Prediction { get; init; }
    public int Stock { get; init; }

    public ProductRecord(int id, int prediction, int stock)
    {
        Id = id;
        Prediction = prediction;
        Stock = stock;
    }
}
