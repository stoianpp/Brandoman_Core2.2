namespace Brandoman.Services.Data
{
    using System.Linq;

    using Brandoman.Data.Common.Repositories;
    using Brandoman.Data.Models;
    using Brandoman.Data.Models.ViewModels;
    using Brandoman.Services.Data.Interfaces;
    using Brandoman.Services.Mapping;

    public class CategoryService : ICategoryService
    {
        private readonly IDeletableEntityRepository<Category> categories;
        private readonly IDeletableEntityRepository<SubCategory> subCategories;

        public CategoryService(IDeletableEntityRepository<Category> categoriesIn, IDeletableEntityRepository<SubCategory> subCategoriesIn)
        {
            this.categories = categoriesIn;
            this.subCategories = subCategoriesIn;
        }

        public IQueryable<CategoryDropDownViewModel> GetAllCategories()
        {
            return this.categories.All().To<CategoryDropDownViewModel>();
        }

        public IQueryable<SubCategoryDropDownViewModel> GetAllSubCategories(int category)
        {
            return this.subCategories.All().Where(x => x.CategoryId == category).To<SubCategoryDropDownViewModel>();
        }

        public int GetCategoryFromSubCategory(int subCategory)
        {
            return this.subCategories.All().FirstOrDefault(x => x.Id == subCategory).CategoryId;
        }

        public int GetInitialCategory()
        {
            return this.categories.All().FirstOrDefault().Id;
        }

        public int? GetInitialSubCategory(int active_category)
        {
            var result = this.subCategories.All().FirstOrDefault(x => x.CategoryId == active_category);
            if (result != null)
            {
                return this.subCategories.All().FirstOrDefault(x => x.CategoryId == active_category).Id;
            }

            return null;
        }

        public string GetSubCategoryName(int cat)
        {
            return this.subCategories.All().FirstOrDefault(x => x.Id == cat).Name;
        }
    }
}
