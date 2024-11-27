using Data.Models;
using FluentValidation;

namespace Core.Validators;

public class ExampleValidator : AbstractValidator<ExampleModel>
{
    public ExampleValidator()
    {
        RuleFor(e => e.Text)
            .NotEmpty().WithMessage("{PropertyName} is required");
        RuleFor(e => e.Translate)
            .NotEmpty().WithMessage("{PropertyName} is required");
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