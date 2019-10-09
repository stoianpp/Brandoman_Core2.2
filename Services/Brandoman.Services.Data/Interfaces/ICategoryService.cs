namespace Brandoman.Services.Data.Interfaces
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Brandoman.Data.Common.Models;
    using Brandoman.Data.Models;
    using Brandoman.Data.Models.ViewModels;
    using Microsoft.AspNetCore.Http;

    public interface ICategoryService
    {
        IQueryable<CategoryDropDownViewModel> GetAllCategories();

        IQueryable<SubCategoryDropDownViewModel> GetAllSubCategories(int category);

        IQueryable<SubCategory> GetAllSubCategories();

        IQueryable<Category> GetAllFullCategories();

        int GetCategoryFromSubCategory(int subCategory);

        int GetInitialCategory();

        int? GetInitialSubCategory(int active_category);

        string GetSubCategoryName(int cat);

        IList<SubCategoryIndexViewModel> GetSubCategoryLangs(IEnumerable<SubCategory> subCategories, Lang lang);

        Task SaveSubCategotyLangAsync(SubCategoryLang subCategoryTranslation);

        SubCategoryLang GetSubCategoryTranslation(int subCategoryLangId);

        bool UpdateSubCategoryTranslation(SubCategoryLang subCategoryTranslation);

        Task SaveSubCategoryAsync(SubCategory subCategory, IFormFile imageName);

        bool UpdateSubCategory(SubCategoryAdminIndexViewModel modelIn, IFormFile imageName);

        List<SubCategory> LocalizeSubCats(List<SubCategory> subCats, Lang lang);
    }
}
