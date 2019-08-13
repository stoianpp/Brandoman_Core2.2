namespace Brandoman.Web.Areas.Administration.Controllers
{
    using Brandoman.Services.Data.Interfaces;
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

        public IActionResult Index()
        {
            var userLang = this.productService.GetCurrentUserLanguage(this.GetUserId());
            var allSubCategories = this.categoryService.GetAllSubCategories();
            var viewModel = this.categoryService.GetSubCategoryLangs(allSubCategories, userLang);
            return this.View(viewModel);
        }
    }
}
