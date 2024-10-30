namespace Domain;

public interface IDataParser
{
    public SaleRecord StringToSaleRecord(string info, int line);
}
