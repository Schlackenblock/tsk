using Dapper;

namespace Tsk.Tests.MigrationTests.SingleMigrationTests;

public class AddProductPicturesMigrationTest : MigrationTestBase
{
    protected override string MigrationUnderTest => "20240804131440_AddProductPictures";

    [Fact]
    public async Task AddProductPictures_WhenApplied_ShouldAutomaticallyCreateEmptyPicturesArrayForExistingProducts()
    {
        var productsBeforeMigration = new ProductBeforeMigration[]
        {
            new(Guid.NewGuid(), "Product #1", 1.99m, true, "P1"),
            new(Guid.NewGuid(), "Product #2", 2.99m, true, "P2"),
            new(Guid.NewGuid(), "Product #3", 3.99m, false, "P3")
        };
        await Connection.ExecuteAsync(
            """
            INSERT INTO products(id, title, price, is_for_sale, code)
            VALUES (@Id, @Title, @Price, @IsForSale, @Code);
            """,
            productsBeforeMigration
        );

        await ApplyTestedMigrationAsync();

        var expectedProductsAfterMigration = productsBeforeMigration.Select(product => new ProductAfterMigration(
            product.Id,
            product.Title,
            product.Price,
            product.IsForSale,
            product.Code,
            Pictures: []
        ));

        var productsAfterMigration = await Connection.QueryAsync<ProductAfterMigration>("SELECT * FROM products;");
        productsAfterMigration.Should().BeEquivalentTo(expectedProductsAfterMigration);
    }

    private record ProductBeforeMigration(
        Guid Id,
        string Title,
        decimal Price,
        bool IsForSale,
        string Code
    );

    private record ProductAfterMigration(
        Guid Id,
        string Title,
        decimal Price,
        bool IsForSale,
        string Code,
        List<string> Pictures
    );
}
