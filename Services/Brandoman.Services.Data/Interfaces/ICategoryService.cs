namespace Brandoman.Services.Data.Interfaces
{
    using System.Linq;
    using Brandoman.Data.Models;
    using Brandoman.Data.Models.ViewModels;

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
    }
}
