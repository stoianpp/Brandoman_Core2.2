namespace Brandoman.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

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
                        SubCategoryId = subCategory.Id,
                    });
                }
                else
                {
                    result.Add(new SubCategoryIndexViewModel
                    {
                        Id = 0,
                        LangText = "No translation available",
                        Name = subCategory.Name,
                        Lang = lang,
                        SubCategoryId = subCategory.Id,
                    });
                }
            }

            return result;
        }

        public async Task SaveSubCategotyLangAsync(SubCategoryLang subCategoryTranslation)
        {
            await this.subCategoryLangs.AddAsync(subCategoryTranslation);
            this.subCategoryLangs.SaveChanges();
        }

        public SubCategoryLang GetSubCategoryTranslation(int subCategoryLangId)
        {
            return this.subCategoryLangs.All().FirstOrDefault(x => x.Id == subCategoryLangId);
        }

        public bool UpdateSubCategoryTranslation(SubCategoryLang subCategoryTranslation)
        {
            bool result;
            try
            {
                this.subCategoryLangs.Update(subCategoryTranslation);
                this.subCategoryLangs.SaveChanges();
                result = true;
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public List<SubCategory> LocalizeSubCats(List<SubCategory> subCats)
        {
            var subCategoryTranslation = this.subCategoryLangs.All();
            foreach (var translation in subCategoryTranslation)
            {
                subCats.FirstOrDefault(x => x.Id == translation.SubCategoryId).Name = translation.LangText;
            }

            return subCats;
        }
    }
}
