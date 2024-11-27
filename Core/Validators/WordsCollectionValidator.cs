
using Data.Models;
using FluentValidation;

namespace Core.Validators;

public class WordsCollectionValidator : AbstractValidator<WordsCollectionModel>
{
    public WordsCollectionValidator()
    {
        RuleFor(wc => wc.UserId)
            .NotEmpty()
            .NotNull()
            .WithMessage("{PropertyName} is required.");
        RuleFor(wc => wc.Language)
            .NotEmpty()
            .NotNull()
            .WithMessage("{PropertyName} is required.");
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