using JetBrains.Annotations;

namespace Tsk.HttpApi.Products;

[PublicAPI]
public record ReadProductDto(Guid Id, string Title, string Description, double Price)
{
    /// <summary>ProductEntity id.</summary>
    /// <example>xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx</example>
    public Guid Id { get; init; } = Id;

    /// <summary>ProductEntity title.</summary>
    /// <example>Schlackenblock.</example>
    public string Title { get; init; } = Title;

    /// <summary>ProductEntity description.</summary>
    /// <example>Solid concrete brick M200 graphite 250x120x65.</example>
    public string Description { get; init; } = Description;

    /// <summary>ProductEntity price.</summary>
    /// <example>1.8</example>
    public double Price { get; init; } = Price;
}

[PublicAPI]
public record CreateProductDto(string Title, string Description, double Price)
{
    /// <summary>ProductEntity title.</summary>
    /// <example>Schlackenblock.</example>
    public string Title { get; init; } = Title;

    /// <summary>ProductEntity description.</summary>
    /// <example>Solid concrete brick M200 graphite 250x120x65.</example>
    public string Description { get; init; } = Description;

    /// <summary>ProductEntity price.</summary>
    /// <example>1.8</example>
    public double Price { get; init; } = Price;
}

[PublicAPI]
public record UpdateProductDto(string Title, string Description, double Price)
{
    /// <summary>ProductEntity title.</summary>
    /// <example>Schlackenblock.</example>
    public string Title { get; init; } = Title;

    /// <summary>ProductEntity description.</summary>
    /// <example>Solid concrete brick M200 graphite 250x120x65.</example>
    public string Description { get; init; } = Description;

    /// <summary>ProductEntity price.</summary>
    /// <example>1.8</example>
    public double Price { get; init; } = Price;
}
