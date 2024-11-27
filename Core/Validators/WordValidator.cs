using Data.Models;
using FluentValidation;

namespace Core.Validators;

public class WordValidator : AbstractValidator<WordModel>
{
    public WordValidator()
    {
        RuleFor(w => w.WordText)
            .NotEmpty().WithMessage("{PropertyName} is required.");
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