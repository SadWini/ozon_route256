using DefaultSite.GrpcService;
using DefaultSite.GrpcServices;

namespace DefaultSite.Validators;
using FluentValidation;

public class FilterRequestValidator:AbstractValidator<FilterRequest>
{
    public FilterRequestValidator()
    {
        DateTime date;
        RuleFor(request => DateTime.TryParse(request.Item.CreatedAt, out date));
        RuleFor(request => request.Item.StorageNumber).GreaterThanOrEqualTo(1);
        RuleFor(request => GrpcMapper.MapToGoodItemType(request.Item.Type));
    }
}
