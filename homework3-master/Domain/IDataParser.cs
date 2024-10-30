namespace Domain;

public interface IDataParser
{
    public ProductRecord StringToProductRecord(string info);
}
