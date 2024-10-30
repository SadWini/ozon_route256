using System.Threading.Channels;
using BusinessLogic;
using Domain;
using Infrastructure;
using Microsoft.Extensions.Configuration;

namespace Application
{
    public class DemandCalculator : IDemandCalculator
    {
        private IProductService _productService;
        private IDataParser _dataParser;
        private int _parallelismDegree = 8;
        private long _processedLines;
        private long _calculatedItems;
        private long _writtenRecords;
        private int _maxCapacity = 10000;
        private int _batchSize = 100;

        private string _configPath =
            "C:\\Users\\user\\RiderProjects\\AdvancedDemandCalculator\\Application\\appsettings.json";
        public DemandCalculator()
        {
            _dataParser = new DataParser();
            _productService = new ProductService();
        }

        public void Init()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(_configPath,
                    optional: true, reloadOnChange: true);
            IConfiguration config = builder.Build();

            _parallelismDegree = int.Parse(config["AppSettings:ParallelismDegree"]);
            _maxCapacity = int.Parse(config["AppSettings:MaxCapacity"]);
            _batchSize = int.Parse(config["AppSettings:BatchSize"]);
        }

        public async Task Job()
        {
            Init();
            while (true)
            {
                Console.WriteLine("Hello! Please specify file with sales data");
                string filePath = Console.ReadLine().Trim();
                try
                {
                    CancellationTokenSource cts = new CancellationTokenSource();
                    Console.CancelKeyPress += (sender, eventArgs) =>
                    {
                        cts.Cancel();
                        eventArgs.Cancel = true;
                    };
                    CancellationToken token = cts.Token;

                    await ProcessFileAsync(filePath,
                        "C:\\Users\\user\\RiderProjects\\AdvancedDemandCalculator\\output.csv",
                        _parallelismDegree, token);
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
                break;
            }
        }

    async Task ProcessFileAsync(string inputFilePath, string outputFilePath, int degreeOfParallelism,
        CancellationToken cancellationToken)
    {
        var channel = Channel.CreateBounded<List<string>>(new BoundedChannelOptions(_maxCapacity));
        var producerTask = ProduceLinesAsync(inputFilePath, channel.Writer, cancellationToken);
        var consumerTasks = Enumerable.Range(0, degreeOfParallelism).Select(i =>
            ConsumeLinesAsync(channel.Reader, outputFilePath, cancellationToken));

        await producerTask;
        channel.Writer.Complete();
        await Task.WhenAll(consumerTasks);
        Console.WriteLine($"total records written: {_writtenRecords}");
    }

    async Task ProduceLinesAsync(string inputFilePath, ChannelWriter<List<string>> writer,
        CancellationToken cancellationToken)
    {
        using (var reader = new StreamReader(inputFilePath))
        {
            string header = await reader.ReadLineAsync();
            var batch = new List<string>(_batchSize);
            while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
            {
                var line = await reader.ReadLineAsync();
                if (line != null)
                {
                    batch.Add(line);
                    Interlocked.Increment(ref _processedLines);
                    Console.WriteLine($"Processed lines {_processedLines}");
                    if (batch.Count == _batchSize)
                    {
                        await writer.WriteAsync(batch, cancellationToken);
                        batch = new List<string>(100);
                    }
                }
            }

            if (batch.Count > 0)
            {
                await writer.WriteAsync(batch, cancellationToken);
            }
        }
    }

    async Task ConsumeLinesAsync(ChannelReader<List<string>> reader, string outputFilePath, CancellationToken cancellationToken)
    {
        using (var writer = new StreamWriter(outputFilePath, append: true))
        {
            await foreach (var batch in reader.ReadAllAsync(cancellationToken))
            {
                foreach (var line in batch)
                {
                    ProductRecord item = _dataParser.StringToProductRecord(line);
                    int demand = await _productService.CalculateDemand(item);
                    Interlocked.Increment(ref _calculatedItems);
                    Console.WriteLine($"Calculated items: {_calculatedItems}");
                    lock (writer)
                    {
                        writer.WriteLine($"{item.Id},{demand}");
                        Interlocked.Increment(ref _writtenRecords);
                    }
                }
                Console.WriteLine($"Processed batch of {batch.Count} lines, total written records: {_writtenRecords}");
            }
        }
    }

        public async static Task Main()
        {
            IDemandCalculator calculator = new DemandCalculator();
            await calculator.Job();
        }
    }
}
