using JetBrains.Annotations;

namespace Tsk.HttpApi.Products;

[PublicAPI]
public record ReadProductDto(
    Guid Id,
    string Title,
    string Name,
    string Type,
    double? Length,
    double? Width,
    double? Height,
    double Price,
    double? Discount
);

[PublicAPI]
public record CreateProductDto(
    string Title,
    string Name,
    string Type,
    double? Length,
    double? Width,
    double? Height,
    double Price,
    double? Discount);

[PublicAPI]
public record UpdateProductDto(
    string Title,
    string Name,
    string Type,
    double? Length,
    double? Width,
    double? Height,
    double Price,
    double? Discount
);
