using System.Net.Http.Json;
using AutoFixture;
using Domain.Dao;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DefaultSite.Tests
{
    public class GoodsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public GoodsControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Test_AddGood_WhenValidItem()
        {
            var client = _factory.CreateClient();
            var fixture = new Fixture();
            var goodItemDto = fixture.Build<GoodItemDto>()
                .With(x => x.Price, 15)
                .With(x => x.Weight, 1)
                .Create();
            var response = await client.PostAsJsonAsync("/addGood", goodItemDto);

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Test_AddGood_WhenInvalidItem()
        {
            var client = _factory.CreateClient();
            var fixture = new Fixture();
            var goodItemDto = fixture.Build<GoodItemDto>()
                .With(x => x.Price, -1)
                .Create();
            var response = await client.PostAsJsonAsync("/addGood", goodItemDto);

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Test_FindById_WhenItemExists()
        {
            var client = _factory.CreateClient();
            var fixture = new Fixture();
            var goodItemDto = fixture.Build<GoodItemDao>()
                .With(x => x.Price, 10)
                .With(x => x.Weight, 2)
                .Create();
            await client.PostAsJsonAsync("/addGood", goodItemDto);

            var response = await client.GetAsync("/find?id=0");

            Assert.True(response.IsSuccessStatusCode);
            var foundItem = await response.Content.ReadFromJsonAsync<GoodItemDto>();
            Assert.NotNull(foundItem);
            Assert.Equal(goodItemDto.Price, foundItem.Price);
        }

        [Fact]
        public async Task Test_UpdateItemPrice_WhenValidUpdate()
        {
            var client = _factory.CreateClient();
            var fixture = new Fixture();
            var goodItemDto = fixture.Build<GoodItemDao>()
                .With(x => x.Price, 10)
                .With(x => x.Weight, 2)
                .Create();
            await client.PostAsJsonAsync("/addGood", goodItemDto);
            var updatePriceDto = fixture.Build<UpdateGoodItemPrice>()
                .With(x => x.Price, goodItemDto.Price * 2)
                .With(x => x.Id, 0)
                .Create();

            var response = await client.PatchAsJsonAsync("/updatePrice", updatePriceDto);

            Assert.True(response.IsSuccessStatusCode);
            var updatedItem = await response.Content.ReadFromJsonAsync<GoodItemDao>();
            Assert.Equal(updatePriceDto.Price, updatedItem?.Price);
        }
    }
}
