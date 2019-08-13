namespace Brandoman.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;

    using Brandoman.Data.Common.Models;
    using Brandoman.Data.Common.Repositories;
    using Brandoman.Data.Models;
    using Brandoman.Data.Models.ViewModels;
    using Brandoman.Services.Data.Interfaces;
    using Brandoman.Services.Mapping;

    public class CategoryService : ICategoryService
    {
        private readonly IDeletableEntityRepository<Category> categories;
        private readonly IDeletableEntityRepository<SubCategory> subCategories;
        private readonly IDeletableEntityRepository<SubCategoryLang> subCategoryLangs;

        public CategoryService(
            IDeletableEntityRepository<Category> categoriesIn,
            IDeletableEntityRepository<SubCategory> subCategoriesIn,
            IDeletableEntityRepository<SubCategoryLang> subCategoryLangs)
        {
            this.categories = categoriesIn;
            this.subCategories = subCategoriesIn;
            this.subCategoryLangs = subCategoryLangs;
        }

        public IQueryable<CategoryDropDownViewModel> GetAllCategories()
        {
            return this.categories.All().To<CategoryDropDownViewModel>();
        }

        public IQueryable<SubCategoryDropDownViewModel> GetAllSubCategories(int category)
        {
            return this.subCategories.All().Where(x => x.CategoryId == category).To<SubCategoryDropDownViewModel>();
        }

        public IQueryable<SubCategory> GetAllSubCategories()
        {
            return this.subCategories.All();
        }

        public IQueryable<Category> GetAllFullCategories()
        {
            return this.categories.All();
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

        public IList<SubCategoryIndexViewModel> GetSubCategoryLangs(IEnumerable<SubCategory> subCategories, Lang lang)
        {
            var subCategoryTranslations = this.subCategoryLangs.All().Where(x => x.Lang == lang);
            var subCategoryTranslationIds = subCategoryTranslations.Select(x => x.SubCategoryId);
            var result = new List<SubCategoryIndexViewModel>();

            foreach (var subCategory in subCategories)
            {
                if (subCategoryTranslationIds.Contains(subCategory.Id))
                {
                    var trans = subCategoryTranslations.FirstOrDefault(x => x.SubCategoryId == subCategory.Id);
                    result.Add(new SubCategoryIndexViewModel
                    {
                        Id = trans.Id,
                        LangText = trans.LangText,
                        Name = trans.Name,
                        Lang = trans.Lang,
                        Image = subCategory.Image,
                    });
                }
                else
                {
                    result.Add(new SubCategoryIndexViewModel
                    {
                        LangText = "No translation available",
                        Name = subCategory.Name,
                        Lang = lang,
                        Image = subCategory.Image,
                    });
                }
            }

            return result;
        }
    }
}
