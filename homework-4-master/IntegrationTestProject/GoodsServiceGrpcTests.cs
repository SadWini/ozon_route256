using DefaultSite.GrpcService;
using Grpc.Core;
using Grpc.Core.Testing;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DefaultSite.Tests;

public class GoodsServiceGrpcTests:IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public GoodsServiceGrpcTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Test_GrpcService_AddGood()
    {
        var item = CreateItem();
        var req = new AddGoodRequest
        {
            GoodItem = item
        };

        var grpClient = new DefaultSite.GrpcServices.GoodsServiceGrpc();
        var response  = await grpClient.AddGood(req, CreateContext());

        Assert.Equal(0, response.ItemId);
    }

    [Fact]
    public async Task Test_GrpcService_FindById()
    {
        var item = CreateItem();
        var req = new AddGoodRequest
        {
            GoodItem = item
        };

        var grpClient = new GrpcServices.GoodsServiceGrpc();
        await grpClient.AddGood(req, CreateContext());
        var findRequest = new FindByIdRequest
        {
            Id = 0
        };
        var response = await grpClient.FindById(findRequest, CreateContext());

        Assert.Equal(response.GoodItem, item);
    }

    [Fact]
    public async Task Test_GrpcService_UpdateItemPrice()
    {
        var item = CreateItem();
        var req = new AddGoodRequest
        {
            GoodItem = item
        };

        var grpClient = new GrpcServices.GoodsServiceGrpc();
        await grpClient.AddGood(req, CreateContext());
        var updateRequest = new UpdateItemPriceRequest()
        {
            Id = 0,
            Price = item.Price * 2
        };
        var response = await grpClient.UpdateItemPrice(updateRequest, CreateContext());

        Assert.Equal(response.Item.Price, item.Price * 2);
    }

    private DefaultSite.GrpcService.GoodItemDto CreateItem()
    {
        var item = new DefaultSite.GrpcService.GoodItemDto();
        item.Name = "name";
        item.Type = ItemType.Common;
        item.Price = 12;
        item.Weight = 3;
        item.StorageNumber = 45;
        item.CreatedAt = DateTime.UtcNow.ToString();

        return item;
    }
    private ServerCallContext CreateContext()
    {
        return TestServerCallContext.Create(
            method: nameof(GrpcServices.GoodsServiceGrpc.AddGood)
            , host: "localhost"
            , deadline: DateTime.Now.AddMinutes(30)
            , requestHeaders: new Metadata()
            , cancellationToken: CancellationToken.None
            , peer: "10.0.0.25:5001"
            , authContext: null
            , contextPropagationToken: null
            , writeHeadersFunc: _ => Task.CompletedTask
            , writeOptionsGetter: () => new WriteOptions()
            , writeOptionsSetter: _ => { }
        );
    }
}
