namespace Brandoman.Services
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Identity;

    public interface ILoginService
    {
        string GetToken(IEnumerable<IdentityUser> user, string username, string password);
    }
}
