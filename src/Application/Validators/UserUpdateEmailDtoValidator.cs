using Application.Dto;
using FluentValidation;

namespace Application.Validators;

public class UserUpdateEmailDtoValidator : AbstractValidator<UserUpdateEmailDto>
{
    public UserUpdateEmailDtoValidator()
    {
        RuleFor(x => x.OldEmail).EmailAddress().MaximumLength(64);
        RuleFor(x => x.NewEmail).EmailAddress().MaximumLength(64);
    }
}