using DefaultSite.Controllers;
using DefaultSite.GrpcService;
using Domain.Dao;
using FluentValidation;
using Grpc.Core;

namespace DefaultSite.GrpcServices;

public class GoodsServiceGrpc : GrpcService.GoodsServiceGrpc.GoodsServiceGrpcBase
{
    private readonly ILogger<GoodsServiceGrpc> _logger;
    private readonly IGoodsRepository _goodsRepository;
    private readonly IValidator<GoodItemDto> _validator;
    private readonly IValidator<UpdateGoodItemPrice> _updateGoodItemValidator;
    private readonly IValidator<AddGoodRequest> _addGoodRequestValidator;
    private readonly IValidator<FilterRequest> _filterRequestValidator;

    public GoodsServiceGrpc(ILogger<GoodsServiceGrpc> logger,
        IGoodsRepository goodsRepository,
        IValidator<GoodItemDto> validator,
        IValidator<UpdateGoodItemPrice> updateGoodItemValidator,
        IValidator<AddGoodRequest> addGoodRequestValidator,
        IValidator<FilterRequest> filterRequestValidator)
    {
        _logger = logger;
        _goodsRepository = goodsRepository;
        _validator = validator;
        _updateGoodItemValidator = updateGoodItemValidator;
        _addGoodRequestValidator = addGoodRequestValidator;
        _filterRequestValidator = filterRequestValidator;
    }

    public override Task<AddGoodResponse> AddGood(AddGoodRequest request, ServerCallContext context)
    {
        var result = _addGoodRequestValidator.Validate(request);
        if (!result.IsValid)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Good Item"));
        }
        var addedItem = _goodsRepository.Add(GrpcMapper.Map(request));

        return Task.FromResult(new AddGoodResponse { ItemGuid = addedItem.ToString() });
    }

    public override Task<FindByIdResponse> FindById(FindByIdRequest request, ServerCallContext context)
    {
        var value = _goodsRepository.Find(Guid.Parse(request.Id));
        return Task.FromResult(new FindByIdResponse{GoodItem = GrpcMapper.Map(value)});
    }

    public override Task<FilterResponse> GetPagedData(FilterRequest request, ServerCallContext context)
    {
        _filterRequestValidator.Validate(request);
        var data = _goodsRepository.Find(GrpcMapper.Map(request))
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var response = new FilterResponse();
        response.Goods.AddRange(data.Select(item => GrpcMapper.Map(item)));

        return Task.FromResult(response);
    }

    public override Task<UpdateItemPriceResponse> UpdateItemPrice(UpdateItemPriceRequest request, ServerCallContext context)
    {
        var updateItem = GrpcMapper.Map(request);
        var validator = _updateGoodItemValidator.Validate(updateItem);
        if (!validator.IsValid)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid update item price"));
        }

        var newItem = _goodsRepository.UpdatePrice(updateItem.Id, updateItem.Price);
        return Task.FromResult(new UpdateItemPriceResponse { Item = GrpcMapper.Map(newItem)});
    }
}
