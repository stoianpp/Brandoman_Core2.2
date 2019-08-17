namespace Brandoman.Services.Data.Interfaces
{
    using System.Linq;
    using System.Threading.Tasks;

    using Brandoman.Data.Common.Models;
    using Brandoman.Data.Models;
    using Brandoman.Data.Models.ViewModels;

    public interface IUserService
    {
        IQueryable<ApplicationUserIndexViewModel> GetUsersByLanguage(Lang lang);

        ApplicationUser GetUserById(string user);

        Task<bool> DeleteUser(ApplicationUser user);

        int GetUsersNumber();
    }
}
