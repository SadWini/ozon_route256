namespace Domain;

public interface IProductService
{
    Task<int> CalculateDemand(ProductRecord record);
}
