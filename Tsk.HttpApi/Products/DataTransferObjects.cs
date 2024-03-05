using JetBrains.Annotations;

namespace Tsk.HttpApi.Products;

[PublicAPI]
public record ReadProductDto(Guid Id, string Title, string Description, double Price)
{
    /// <summary>Product id.</summary>
    /// <example>xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx</example>
    public Guid Id { get; init; } = Id;

    /// <summary>Product title.</summary>
    /// <example>Schlackenblock.</example>
    public string Title { get; init; } = Title;

    /// <summary>Product description.</summary>
    /// <example>Solid concrete brick M200 graphite 250x120x65.</example>
    public string Description { get; init; } = Description;

    /// <summary>Product price.</summary>
    /// <example>1.8</example>
    public double Price { get; init; } = Price;
}

[PublicAPI]
public record CreateProductDto(string Title, string Description, double Price)
{
    /// <summary>Product title.</summary>
    /// <example>Schlackenblock.</example>
    public string Title { get; init; } = Title;

    /// <summary>Product description.</summary>
    /// <example>Solid concrete brick M200 graphite 250x120x65.</example>
    public string Description { get; init; } = Description;

    /// <summary>Product price.</summary>
    /// <example>1.8</example>
    public double Price { get; init; } = Price;
}

[PublicAPI]
public record UpdateProductDto(string Title, string Description, double Price)
{
    /// <summary>Product title.</summary>
    /// <example>Schlackenblock.</example>
    public string Title { get; init; } = Title;

    /// <summary>Product description.</summary>
    /// <example>Solid concrete brick M200 graphite 250x120x65.</example>
    public string Description { get; init; } = Description;

    /// <summary>Product price.</summary>
    /// <example>1.8</example>
    public double Price { get; init; } = Price;
}
