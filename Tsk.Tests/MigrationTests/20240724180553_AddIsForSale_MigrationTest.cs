using Dapper;

namespace Tsk.Tests.MigrationTests;

public class AddIsForSaleMigrationTest : MigrationTestBase
{
    private const string previousMigration = "20240404140221_RemoveProductDescription";
    private const string migrationUnderTest = "20240724180553_AddIsForSale";

    [Fact]
    public async Task AddIsForSaleMigration_WhenApplied_ShouldPreserveExistingProductsAndMarkThemAsForSale()
    {
        await using var connection = Database.GetDbConnection();

        await Migrator.MigrateAsync(previousMigration);

        var productsBeforeMigration = new ProductBeforeMigration[]
        {
            new(Guid.NewGuid(), "Product #1", 1.99),
            new(Guid.NewGuid(), "Product #2", 2.99),
            new(Guid.NewGuid(), "Product #3", 3.99)
        };
        await connection.ExecuteAsync(
            """
            INSERT INTO products(id, title, price)
            VALUES (@Id, @Title, @Price);
            """,
            productsBeforeMigration
        );

        await Migrator.MigrateAsync(migrationUnderTest);

        var expectedProductsAfterMigration = productsBeforeMigration.Select(product => new ProductAfterMigration(
            product.Id,
            product.Title,
            product.Price,
            IsForSale: true
        ));

        var productsAfterMigration = await connection.QueryAsync<ProductAfterMigration>("SELECT * FROM products;");
        productsAfterMigration.Should().BeEquivalentTo(expectedProductsAfterMigration);
    }

    private record ProductBeforeMigration(Guid Id, string Title, double Price);

    private record ProductAfterMigration(Guid Id, string Title, double Price, bool IsForSale);
}
