using DefaultSite.GrpcService;
using FluentValidation;

public class ItemValidator:AbstractValidator<GoodItemDto>
{
    public ItemValidator()
    {
        RuleFor(x=> x.Price).GreaterThan(0);
        RuleFor(x => x.Weight).GreaterThan(0);
        RuleFor(x => x.Name).NotNull().NotEmpty();
    }
}
