using System.Collections.ObjectModel;
using Domain;
using Presentation;

public class SalesRepository : ISalesRepository
{
    private Dictionary<int, List<SaleRecord>> _saleRecords;
    private IDataParser _dataParser;

    public SalesRepository(IDataParser dataParser, string filePath)
    {
        _dataParser = dataParser;
        _saleRecords = FillRepository(filePath);
    }

    private Dictionary<int, List<SaleRecord>> FillRepository(string filePath)
    {
        Dictionary<int, List<SaleRecord>> saleRecords = new Dictionary<int, List<SaleRecord>>();
        string[] lines = File.ReadAllLines(filePath);

        for (int i = 1; i < lines.Length; i++)
        {
            SaleRecord sale = _dataParser.StringToSaleRecord(lines[i], i);
            if (!saleRecords.ContainsKey(sale.Id))
                saleRecords.Add(sale.Id, new List<SaleRecord>());
            saleRecords[sale.Id].Add(sale);
        }
        return saleRecords;
    }

    public IReadOnlyList<SaleRecord> GetSalesById(int id)
    {
        if (!_saleRecords.ContainsKey(id))
            throw new NotFoundException($"Item with ID = {id} doesn't exist");
        return new ReadOnlyCollection<SaleRecord>(_saleRecords[id]);
    }
}
