namespace Domain;

public interface ISalesService
{
    double CalculateADS(int id);
    int PredictSales(int id, int days);
    int CalculateDemand(int id, int days);
}
