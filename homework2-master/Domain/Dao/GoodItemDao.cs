namespace Domain.Dao;

public class GoodItemDao
{
    public Guid Id { get;set; }
    public string Name { get; set; }
    public decimal Weight { get; set; }
    public decimal Price { get;set; }
    public GoodItemType Type { get; set; }
    public DateTime CreatedAt { get;set; }
    public int StorageNumber { get; set; }
}
