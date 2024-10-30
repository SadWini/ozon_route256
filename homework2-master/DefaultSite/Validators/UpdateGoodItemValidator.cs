using FluentValidation;

public class UpdateGoodItemValidator:AbstractValidator<UpdateGoodItemPrice>
{
    public UpdateGoodItemValidator()
    {
        RuleFor(x=> x.Price).GreaterThan(0);
    }
}
