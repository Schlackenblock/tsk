using Dapper;

namespace Tsk.Tests.MigrationTests;

public class AddProductCodeMigrationTest : MigrationTestBase
{
    private const string previousMigration = "20240726153710_FixProductsPKName";
    private const string migrationUnderTest = "20240726193205_AddProductCode";

    [Fact]
    public async Task AddProductCode_WhenApplied_ShouldAutomaticallyCreateProductCodesFromTheirIds()
    {
        await using var connection = Database.GetDbConnection();

        await Migrator.MigrateAsync(previousMigration);

        var productsBeforeMigration = new ProductBeforeMigration[]
        {
            new(Guid.NewGuid(), "Product #1", 1.99m, true),
            new(Guid.NewGuid(), "Product #2", 2.99m, true),
            new(Guid.NewGuid(), "Product #3", 3.99m, false)
        };
        await connection.ExecuteAsync(
            """
            INSERT INTO products(id, title, price, is_for_sale)
            VALUES (@Id, @Title, @Price, @IsForSale);
            """,
            productsBeforeMigration
        );

        await Migrator.MigrateAsync(migrationUnderTest);

        var expectedProductsAfterMigration = productsBeforeMigration.Select(product => new ProductAfterMigration(
            product.Id,
            product.Title,
            product.Price,
            product.IsForSale,
            Code: product.Id.ToString()
        ));

        var productsAfterMigration = await connection.QueryAsync<ProductAfterMigration>("SELECT * FROM products;");
        productsAfterMigration.Should().BeEquivalentTo(expectedProductsAfterMigration);
    }

    private record ProductBeforeMigration(Guid Id, string Title, decimal Price, bool IsForSale);

    private record ProductAfterMigration(Guid Id, string Title, decimal Price, bool IsForSale, string Code);
}
