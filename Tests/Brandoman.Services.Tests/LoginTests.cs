using Brandoman.Data.Common.Repositories;
using Brandoman.Data.Models;
using Moq;
using System;
using Brandoman.Services;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Brandoman.Data.Common.Models;
using Microsoft.Extensions.Options;

namespace Brandoman.Services.Tests
{
    public class LoginTests
    {
        [Fact]
        public void CheckIfLoginsAreRecorderOnEachWebapiSuccessfullLogin()
        {
            var repository = new Mock<IDeletableEntityRepository<LoginLog>>();
            var token = new Mock<IOptions<JwtSettings>>();
            repository.Setup(r => r.All()).Returns(new List<LoginLog>
                                                        {
                                                            new LoginLog{ CreatedOn = DateTime.Now.AddYears(-10) },
                                                            new LoginLog{ CreatedOn = DateTime.Now.AddYears(-5) },
                                                            new LoginLog{ CreatedOn = DateTime.Now },
                                                            new LoginLog{ CreatedOn = DateTime.Now },
                                                            new LoginLog{ CreatedOn = DateTime.Now.AddYears(5) },
                                                            new LoginLog{ CreatedOn = DateTime.Now.AddYears(10) },
                                                        }.AsQueryable());

            var service = new LoginService(token.Object, repository.Object);
            Assert.Equal(6, service.All().Count());
        }
    }
}
