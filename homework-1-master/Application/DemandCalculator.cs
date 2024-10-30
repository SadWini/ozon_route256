using BusinessLogic;
using Domain;
using Presentation;

public class DemandCalculator : IDemandCalculator
{
    private ISalesRepository _salesRepository;
    private ISalesService _salesService;
    private ICommandParser _commandParser;
    private IDataParser _dataParser;
    public DemandCalculator()
    {
        _commandParser = new CommandParser();
        _dataParser = new DataParser();
    }

    private void Init()
    {
        while (true)
        {
            Console.WriteLine("Hello! Please specify file with sales data");
            string filePath = Console.ReadLine().Trim();
            try
            {
                _salesRepository = new SalesRepository(_dataParser, filePath);
                _salesService = new SalesService(_salesRepository);
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Check data format of file {filePath}, {ex.Message}");
                continue;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Check file path {filePath}, {ex.Message}");
                continue;
            }
            return;
        }
    }

    public void Job()
    {
        Init();
        while (true)
        {
            PrintCommands();
            string input = Console.ReadLine().Trim().ToLower();
            CommandRawParseResult res;
            int id, days;
            try
            {
                res = _commandParser.Parse(input);
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Invalid command, {ex.Message}");
                continue;
            }

            try
            {
                switch (res.Type)
                {
                    case CommandType.ADS:
                        Console.WriteLine(_salesService.CalculateADS(res.arguments[0]));
                        break;
                    case CommandType.PREDICTION:
                        Console.WriteLine(_salesService.PredictSales(res.arguments[0], res.arguments[1]));
                        break;
                    case CommandType.DEMAND:
                        Console.WriteLine(_salesService.CalculateDemand(res.arguments[0], res.arguments[1]));
                        break;
                    default:
                        Console.WriteLine("Didn't recognize command, please try another time");
                        break;
                }
            }
            catch (NotFoundException ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }
    }

    private static void PrintCommands()
    {
        Console.WriteLine($"Now you can use the next commands: {Environment.NewLine} " +
                          $"ads <ID item> to calculate ads of item with ID {Environment.NewLine}" +
                          $"prediction <ID item> <number of days> to calculate predict of sales {Environment.NewLine}" +
                          $"demand <ID item> <number of days> to calculate demand of item with ID");
    }

    public static void Main()
    {
        IDemandCalculator calculator = new DemandCalculator();
        calculator.Job();
    }
}
