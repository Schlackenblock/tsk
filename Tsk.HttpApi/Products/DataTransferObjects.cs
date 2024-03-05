using JetBrains.Annotations;

namespace Tsk.HttpApi.Products;

[PublicAPI]
public record ReadProductDto(
    Guid Id,
    string Title,
    string Name,
    double Price
);

[PublicAPI]
public record CreateProductDto(
    string Title,
    string Name,
    double Price
);

[PublicAPI]
public record UpdateProductDto(
    string Title,
    string Name,
    double Price
);
