using Tsk.HttpApi.Entities;
using Tsk.HttpApi.Products.ForAdmins;

namespace Tsk.Tests.Products;

public static class ProductModelsConverter
{
    public static CreateProductDto ToCreateProductDto(this Product product)
    {
        return new CreateProductDto
        {
            Code = product.Code,
            Title = product.Title,
            Pictures = product.Pictures,
            Price = product.Price
        };
    }
}
