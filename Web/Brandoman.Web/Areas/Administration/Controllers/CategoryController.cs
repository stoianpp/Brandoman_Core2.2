namespace Brandoman.Web.Areas.Administration.Controllers
{
    using Brandoman.Common;
    using Brandoman.Data.Models;
    using Brandoman.Data.Models.ViewModels;
    using Brandoman.Services.Data.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class CategoryController : AdministrationController
    {
        private readonly ICategoryService categoryService;
        private readonly IProductService productService;

        public CategoryController(ICategoryService categoryService, IProductService productService)
        {
            this.categoryService = categoryService;
            this.productService = productService;
        }

        public IActionResult Index(string toastr)
        {
            var userLang = this.productService.GetCurrentUserLanguage(this.GetUserId());
            var allSubCategories = this.categoryService.GetAllSubCategories();
            var viewModel = this.categoryService.GetSubCategoryLangs(allSubCategories, userLang);

            this.ViewBag.Title = "Category Translations";
            this.ViewBag.Toastr = toastr;

            return this.View(viewModel);
        }

        [Route("Category/AddEditCategoryTranslation")]
        [Authorize(Roles = GlobalConstants.LocalAdministratorRoleName)]
        public IActionResult AddEditCategoryTranslation(SubCategoryIndexViewModel modelIn)
        {
            if (this.ModelState.IsValid)
            {
                if (modelIn.Id != 0)
                {
                    this.ViewBag.Title = "Edit Translation";
                }
                else
                {
                    this.ViewBag.Title = "New Translation";
                }

                if (modelIn.LangText == "No translation available")
                {
                    modelIn.LangText = string.Empty;
                }

                return this.View(modelIn);
            }

            return this.RedirectToAction("Index");
        }

        [ActionName("AddEditCategoryTranslations")]
        [HttpPost]
        [Authorize(Roles = GlobalConstants.LocalAdministratorRoleName)]
        public IActionResult AddEditCategoryTranslations(SubCategoryIndexViewModel modelIn)
        {
            if (this.ModelState.IsValid)
            {
                if (modelIn.Id == 0)
                {
                    try
                    {
                        var subCategoryTranslation = new SubCategoryLang
                        {
                            Name = modelIn.Name,
                            LangText = modelIn.LangText,
                            SubCategoryId = modelIn.SubCategoryId,
                            Lang = modelIn.Lang,
                        };
                        this.categoryService.SaveSubCategotyLangAsync(subCategoryTranslation);
                        return this.RedirectToAction("Index", "Category", new { toastr = "New local category name has been created successfully." });
                    }
                    catch
                    {
                        return this.RedirectToAction("Index", "Category", new { toastr = "ERROR: No category translation has been created. Try again." });
                    }
                }
                else
                {
                    try
                    {
                        var subCategoryTranslation = this.categoryService.GetSubCategoryTranslation((int)modelIn.Id);
                        subCategoryTranslation.LangText = modelIn.LangText;
                        this.categoryService.UpdateSubCategoryTranslation(subCategoryTranslation);
                        return this.RedirectToAction("Index", "Category", new { toastr = "The categroy translation has been updated successfully" });
                    }
                    catch
                    {
                        return this.RedirectToAction("Index", "Category", new { toastr = "ERROR: Category translation hasn't been updated. Try again." });
                    }
                }
            }

            this.ViewBag.Message = modelIn.Id != 0 ? "Edit Product" : "New Product";
            return this.View("AddEditCategoryTranslation", modelIn);
        }
    }
}
