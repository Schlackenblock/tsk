using Dapper;

namespace Tsk.Tests.MigrationTests.SingleMigrationTests;

public class AddIsForSaleMigrationTest : MigrationTestBase
{
    protected override string MigrationUnderTest => "20240724180553_AddIsForSale";

    [Fact]
    public async Task AddIsForSaleMigration_WhenApplied_ShouldPreserveExistingProductsAndMarkThemAsForSale()
    {
        var productsBeforeMigration = new ProductBeforeMigration[]
        {
            new(Guid.NewGuid(), "Product #1", 1.99),
            new(Guid.NewGuid(), "Product #2", 2.99),
            new(Guid.NewGuid(), "Product #3", 3.99)
        };
        await Connection.ExecuteAsync(
            """
            INSERT INTO products(id, title, price)
            VALUES (@Id, @Title, @Price);
            """,
            productsBeforeMigration
        );

        await ApplyTestedMigrationAsync();

        var expectedProductsAfterMigration = productsBeforeMigration.Select(product => new ProductAfterMigration(
            product.Id,
            product.Title,
            product.Price,
            IsForSale: true
        ));

        var productsAfterMigration = await Connection.QueryAsync<ProductAfterMigration>("SELECT * FROM products;");
        productsAfterMigration.Should().BeEquivalentTo(expectedProductsAfterMigration);
    }

    private record ProductBeforeMigration(Guid Id, string Title, double Price);

    private record ProductAfterMigration(Guid Id, string Title, double Price, bool IsForSale);
}
