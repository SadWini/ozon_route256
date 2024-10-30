
using Domain.Dao;

using Swashbuckle.AspNetCore.Annotations;

[SwaggerSchema]
public class GoodItemDto
{
    [SwaggerSchema("Цена продукта")]
    public decimal Price {get;set;}

    [SwaggerSchema("Название продукта")]
    public string Name { get; set; }

    [SwaggerSchema("Вес продукта")]
    public decimal Weight { get; set; }

    [SwaggerSchema("Категория продукта")]
    public GoodItemType Type { get; set; }

    [SwaggerSchema("Дата создания продукта")]
    public DateTime CreatedAt { get;set; }

    [SwaggerSchema("Номер склада, где хранится продукт")]
    public int StorageNumber { get; set; }

}
