using Application.Dto;
using FluentValidation;

namespace Application.Validators;

public class UserRegisterDtoValidator : AbstractValidator<UserRegisterDto>
{
    public UserRegisterDtoValidator()
    {
        RuleFor(x => x.Email).EmailAddress().MaximumLength(64);
        RuleFor(x => x.FirstName).MinimumLength(3).MaximumLength(64);
        RuleFor(x => x.LastName).MinimumLength(3).MaximumLength(64);
        RuleFor(x => x.BirthDate).LessThan(DateTime.Today);
    }
}