using Domain;

namespace BusinessLogic;

public class ProductService : IProductService
{
    public async Task<int> CalculateDemand(ProductRecord product)
    {
        Thread.Sleep(10);
        return Math.Max(0, product.Prediction - product.Stock);
    }
}
