namespace Domain;

public interface ISalesRepository
{
    IReadOnlyList<SaleRecord> GetSalesById(int id);
}
