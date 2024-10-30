using Domain.Dao;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DefaultSite.Controllers;

[ApiController]
[Route("/api/v1/[controller]")]
public class GoodsController : ControllerBase
{
    private readonly ILogger<GoodsController> _logger;
    private readonly IGoodsRepository _goodsRepository;
    private readonly IValidator<GoodItemDto> _validator;
    private readonly IValidator<UpdateGoodItemPrice> _updateGoodItemValidator;
    private readonly int _timeout;

    public GoodsController(ILogger<GoodsController> logger,
        IGoodsRepository goodsRepository,
        IValidator<GoodItemDto> validator,
        IValidator<UpdateGoodItemPrice> updateGoodItemValidator)
    {
        _logger = logger;
        _goodsRepository = goodsRepository;
        _validator = validator;
        _updateGoodItemValidator= updateGoodItemValidator;
    }

    [HttpPost]
    [SwaggerOperation("метод добавления товара")]
    [Route("/addGood")]
    public IActionResult AddGood(GoodItemDto goodItemDto)
    {
        var result = _validator.Validate(goodItemDto);
        if(!result.IsValid)
        {
            return BadRequest();
        }

        var addedItem = _goodsRepository.Add(new Domain.Dao.GoodItemDao()
        {
            Price = goodItemDto.Price,
            Name = goodItemDto.Name,
            Weight = goodItemDto.Weight,
            CreatedAt = goodItemDto.CreatedAt,
            Type = goodItemDto.Type,
            StorageNumber = goodItemDto.StorageNumber
        });
        return Ok(addedItem);
    }

    [HttpGet]
    [SwaggerOperation("метод поиска товара по id")]
    [Route("/find")]
    public IActionResult FindById(Guid id){
        var value = _goodsRepository.Find(id);
        return Ok(new GoodItemDto()
        {
            Price = value.Price,
            Name = value.Name,
            Weight = value.Weight,
            CreatedAt = value.CreatedAt,
            Type = value.Type,
            StorageNumber = value.StorageNumber
        });
    }

    [HttpGet]
    [SwaggerOperation("метод поиска товара с фильтрами")]
    [Route("/filter")]
    public IActionResult GetPagedData(Filter filter, int pageNumber = 1, int pageSize = 10)
    {
        var data = _goodsRepository.Find(filter)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        return Ok(data);
    }

    [HttpPatch]
    [SwaggerOperation("метод обновления цены товара")]
    [Route("/updatePrice")]
    public IActionResult UpdateItemPrice(UpdateGoodItemPrice item)
    {
        var validator = _updateGoodItemValidator.Validate(item);
        if (!validator.IsValid)
        {
            return BadRequest();
        }
        return Ok(_goodsRepository.UpdatePrice(item.Id, item.Price));
    }
}
