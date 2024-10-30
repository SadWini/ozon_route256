namespace DefaultSite.Controllers.Dto;

public class Filter
{
    public DateTime CreatedAt { get; set; }
    public Guid StorageNumber { get; set; }
    public GoodItemTypeDto ItemType { get; set; }
}
