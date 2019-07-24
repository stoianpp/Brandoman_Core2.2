namespace Brandoman.Services.Data.Interfaces
{
    using System.Linq;

    using Brandoman.Data.Models;
    using Brandoman.Data.Models.ViewModels;

    public interface IProductService
    {
        IQueryable<AdminIndexViewModel> GetAllAdminActiveProducts(int active_subCategory);

        Product GetProductById(int id);
    }
}
