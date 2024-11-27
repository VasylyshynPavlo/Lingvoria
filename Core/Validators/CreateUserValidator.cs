using Data.Models;
using FluentValidation;

namespace Core.Validators;

public class CreateUserValidator : AbstractValidator<CreateUserModel>
{
    public CreateUserValidator()
    {
        RuleFor(u => u.Username)
            .NotEmpty()
            .NotNull()
            .MinimumLength(5)
            .MaximumLength(20)
            .WithMessage("Username must be between 5 and 20 characters");
        RuleFor(u => u.FullName)
            .MinimumLength(5).WithMessage("{PropertyName} must contain at least 5 characters.");
        RuleFor(u => u.Email)
            .NotEmpty()
            .NotNull()
            .EmailAddress().WithMessage("Email address is not valid");
    }
    
    private static bool LinkMustBeAUri(string? link)
    {
        if (string.IsNullOrWhiteSpace(link))
        {
            return false;
        }
        
        Uri outUri;
        return Uri.TryCreate(link, UriKind.Absolute, out outUri)
               && (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps);
    }
}