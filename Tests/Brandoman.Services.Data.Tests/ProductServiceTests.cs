namespace Brandoman.Services.Data.Tests
{
    using System.Linq;

    using Brandoman.Data;
    using Brandoman.Data.Models;
    using Brandoman.Data.Repositories;
    using Ganss.XSS;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    public class ProductServiceTests
    {
        [Fact]
        public async void CreateNewProductAsync()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Product_Database")
                .Options;
            var dbContext = new ApplicationDbContext(options);
            await dbContext.AddAsync(new Product { Name = "TestProduct", Details = "TestDetails" });
            await dbContext.SaveChangesAsync();

            var product = new EfDeletableEntityRepository<Product>(dbContext);
            var productLang = new EfDeletableEntityRepository<ProductLang>(dbContext);
            var userStore = new UserStore<ApplicationUser>(dbContext);
            var sanitizer = new HtmlSanitizer();

            var service = new ProductService(product, productLang, userStore, sanitizer);
            var count = service.GetAll().Count();
            Assert.Equal(1, count);
        }

        [Fact]
        public async void DeleteProductAsync()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Product_Database")
                .Options;
            var dbContext = new ApplicationDbContext(options);
            await dbContext.AddAsync(new Product { Name = "TestProduct", Details = "TestDetails" });
            await dbContext.SaveChangesAsync();

            var product = new EfDeletableEntityRepository<Product>(dbContext);
            var productLang = new EfDeletableEntityRepository<ProductLang>(dbContext);
            var userStore = new UserStore<ApplicationUser>(dbContext);
            var sanitizer = new HtmlSanitizer();

            var service = new ProductService(product, productLang, userStore, sanitizer);

            var productToDelete = service.GetAll().FirstOrDefault(x => x.Details == "TestDetails");
            await service.Delete(productToDelete);
            var count = service.GetAll().Count();
            Assert.Equal(0, count);
        }
    }
}
