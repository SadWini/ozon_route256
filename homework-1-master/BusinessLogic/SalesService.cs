using Domain;

namespace BusinessLogic;

public class SalesService : ISalesService
{
    private ISalesRepository _salesRepository;

    public SalesService(ISalesRepository salesRepository)
    {
        _salesRepository = salesRepository;
    }

    public double CalculateADS(int id)
    {
        var saleRecords = _salesRepository.GetSalesById(id)
            .Where(x => x.Sales > 0 || x.Stock > 0)
            .ToList();
        if (saleRecords.Count == 0) return 0;
        return saleRecords.Sum(x => x.Sales) / (double) saleRecords.Count;
    }

    public int PredictSales(int id, int days)
    {
        return (int) CalculateADS(id) * days;
    }

    public int CalculateDemand(int id, int days)
    {
        var saleRecords = _salesRepository.GetSalesById(id);
        var lastDateTime = saleRecords.Max(x => x.Date);
        var stock = saleRecords.Where(x => x.Date == lastDateTime).
            Min(x => x.Stock);
        return PredictSales(id, days) - stock;
    }
}
