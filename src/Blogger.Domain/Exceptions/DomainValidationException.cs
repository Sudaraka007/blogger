namespace Blogger.Domain.Exceptions;

public sealed class DomainValidationException : Exception
{
    public DomainValidationException(string propertyName, string message)
        : this([new DomainValidationFailure(propertyName, message)])
    {
    }

    public DomainValidationException(IReadOnlyList<DomainValidationFailure> failures)
        : base("Validation failed.")
    {
        Failures = failures;
    }

    public IReadOnlyList<DomainValidationFailure> Failures { get; }
}

public sealed record DomainValidationFailure(string PropertyName, string ErrorMessage);
