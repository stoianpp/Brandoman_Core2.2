namespace Brandoman.Services.Data.Interfaces
{
    using System.Linq;

    using Brandoman.Data.Common.Models;
    using Brandoman.Data.Models.ViewModels;

    public interface IUserService
    {
        IQueryable<ApplicationUserIndexViewModel> GetUsersByLanguage(Lang lang);
    }
}
