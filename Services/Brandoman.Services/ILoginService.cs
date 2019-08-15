namespace Brandoman.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Brandoman.Data.Models;
    using Microsoft.AspNetCore.Identity;

    public interface ILoginService
    {
        string GetToken(IEnumerable<IdentityUser> user, string username, string password);

        Task LoginRecord(LoginLog login);
    }
}
