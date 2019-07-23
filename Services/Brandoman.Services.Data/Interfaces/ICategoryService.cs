namespace Brandoman.Services.Data.Interfaces
{
    using System.Linq;

    using Brandoman.Data.Models.ViewModels;

    public interface ICategoryService
    {
        IQueryable<CategoryDropDownViewModel> GetAllCategories();

        IQueryable<SubCategoryDropDownViewModel> GetAllSubCategories(int category);

        int GetCategoryFromSubCategory(int subCategory);

        int GetInitialCategory();

        int? GetInitialSubCategory(int active_category);
    }
}
