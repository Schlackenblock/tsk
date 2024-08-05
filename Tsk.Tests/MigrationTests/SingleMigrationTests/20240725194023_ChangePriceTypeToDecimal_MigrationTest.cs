using Dapper;

namespace Tsk.Tests.MigrationTests.SingleMigrationTests;

public class ChangePriceTypeToDecimalMigrationTest : MigrationTestBase
{
    protected override string MigrationUnderTest => "20240725194023_ChangePriceTypeToDecimal";

    [Fact]
    public async Task ChangePriceTypeToDecimal_WhenApplied_ShouldProperlyTransformPrices()
    {
        var productsBeforeMigration = new ProductBeforeMigration[]
        {
            new(Guid.NewGuid(), "Product for Sale", 1.99, true),
            new(Guid.NewGuid(), "Product not for Sale", 2.99, false),
            new(Guid.NewGuid(), "Round price down", 1.004, true),
            new(Guid.NewGuid(), "Round price up in corner-case", 1.005, true),
            new(Guid.NewGuid(), "Round price up", 1.006, true)
        };
        await Connection.ExecuteAsync(
            """
            INSERT INTO products(id, title, price, is_for_sale)
            VALUES (@Id, @Title, @Price, @IsForSale);
            """,
            productsBeforeMigration
        );

        await ApplyTestedMigrationAsync();

        var expectedProductsAfterMigration = productsBeforeMigration.Select(product => new ProductAfterMigration(
            product.Id,
            product.Title,
            Math.Round((decimal)product.Price, 2, MidpointRounding.AwayFromZero),
            product.IsForSale
        ));

        var productsAfterMigration = await Connection.QueryAsync<ProductAfterMigration>("SELECT * FROM products;");
        productsAfterMigration.Should().BeEquivalentTo(expectedProductsAfterMigration);
    }

    private record ProductBeforeMigration(Guid Id, string Title, double Price, bool IsForSale);

    private record ProductAfterMigration(Guid Id, string Title, decimal Price, bool IsForSale);
}
