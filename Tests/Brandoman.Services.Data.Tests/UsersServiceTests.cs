namespace Brandoman.Services.Data.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Brandoman.Data;
    using Brandoman.Data.Common.Repositories;
    using Brandoman.Data.Models;
    using Brandoman.Data.Repositories;
    using Brandoman.Services.Data.Interfaces;
    using Microsoft.EntityFrameworkCore;

    using Moq;

    using Xunit;

    public class UsersServiceTests
    {
        [Fact]
        public void GetCountShouldReturnCorrectNumber()
        {
            var repository = new Mock<IDeletableEntityRepository<ApplicationUser>>();
            repository.Setup(r => r.All()).Returns(new List<ApplicationUser>
                                                        {
                                                            new ApplicationUser(),
                                                            new ApplicationUser(),
                                                            new ApplicationUser(),
                                                        }.AsQueryable());
            var service = new UserService(repository.Object);
            Assert.Equal(3, service.GetUsersNumber());
            repository.Verify(x => x.All(), Times.Once);
        }

        [Fact]
        public async Task GetCountShouldReturnCorrectNumberUsingDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Find_User_Database") // Give a Unique name to the DB
                .Options;
            var dbContext = new ApplicationDbContext(options);
            dbContext.Users.Add(new ApplicationUser());
            dbContext.Users.Add(new ApplicationUser());
            dbContext.Users.Add(new ApplicationUser());
            await dbContext.SaveChangesAsync();

            var repository = new EfDeletableEntityRepository<ApplicationUser>(dbContext);
            var service = new UserService(repository);
            var count = service.GetUsersNumber();
            Assert.Equal(3, count);
        }
    }
}
