using DefaultSite.GrpcService;
using FluentValidation;

namespace DefaultSite.Validators;

public class AddGoodRequestValidator : AbstractValidator<AddGoodRequest>
{
    public AddGoodRequestValidator()
    {
        RuleFor(request => request.GoodItem.Price).GreaterThan(0);
        RuleFor(request => request.GoodItem.Weight).GreaterThan(0);
        RuleFor(request => request.GoodItem.Weight).NotNull().NotEmpty();
    }
}
