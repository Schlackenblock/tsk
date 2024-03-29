using JetBrains.Annotations;

namespace Tsk.HttpApi.Products;

[PublicAPI]
public record ProductDto(Guid Id, string Title, string Description, double Price);

[PublicAPI]
public record CreateProductDto(string Title, string Description, double Price);

[PublicAPI]
public record UpdateProductDto(string Title, string Description, double Price);
