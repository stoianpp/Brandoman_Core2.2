namespace Brandoman.Services.Data.Interfaces
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Brandoman.Data.Common.Models;
    using Brandoman.Data.Models;
    using Brandoman.Data.Models.ViewModels;
    using Microsoft.AspNetCore.Http;

    public interface IProductService
    {
        IQueryable<AdminIndexViewModel> GetAllAdminActiveProducts(int active_subCategory);

        IQueryable<LocalAdminIndexViewModel> GetAllLocalAdminActiveProducts(int active_subCategory, Lang userLanguage);

        Product GetProductById(int id);

        Task SaveProductAsync(Product product, IFormFile imageName);

        Task SaveTranslationAsync(TranslationViewModel translationIn);

        IQueryable<Product> GetAll();

        Task<bool> Delete(Product toDelete);

        void UpdateAsync(ProductViewModel productVM, IFormFile imageName);

        bool Update(ProductViewModel productVM, IFormFile imageName);

        bool UpdateTranslation(TranslationViewModel translationIn);

        void MultipleUpdate(IEnumerable<OrderViewModel> orders);

        Lang GetCurrentUserLanguage(string userId);

        TranslationViewModel GetTranslationById(int id);

        TranslationViewModel GetNewTranslation(int cat, int productId, int? id, Lang lang, Product product, string subCategory);

        IQueryable<EndUserViewModel> GetEndUserIndexData(int cat, Lang lang);
    }
}
