namespace Brandoman.Services.Data.Interfaces
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Brandoman.Data.Common.Models;
    using Brandoman.Data.Models;
    using Brandoman.Data.Models.ViewModels;

    public interface IUserService
    {
        IQueryable<ApplicationUserIndexViewModel> GetUsersByLanguage(Lang lang);

        IQueryable<GlobalApplicationUserViewModel> GetLocalAdminUsers();

        ApplicationUser GetUserById(string user);

        Task<bool> DeleteUser(ApplicationUser user);

        int GetUsersNumber();

        IList<string> GetRolesForCurrentUser(string userId);
    }
}
