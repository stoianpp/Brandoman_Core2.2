namespace Brandoman.Services.Data.Interfaces
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Brandoman.Data.Models;
    using Brandoman.Data.Models.ViewModels;
    using Microsoft.AspNetCore.Http;

    public interface IProductService
    {
        IQueryable<AdminIndexViewModel> GetAllAdminActiveProducts(int active_subCategory);

        Product GetProductById(int id);

        Task SaveProductAsync(Product product, IFormFile imageName);

        IQueryable<Product> GetAll();

        Task<bool> Delete(Product toDelete);

        void UpdateAsync(ProductViewModel productVM, IFormFile imageName);

        bool Update(ProductViewModel productVM, IFormFile imageName);

        void MultipleUpdate(IEnumerable<OrderViewModel> orders);
    }
}
