namespace Brandoman.Web.Controllers
{
    using System.Security.Claims;

    using Brandoman.Common;
    using Brandoman.Data.Models;
    using Brandoman.Services.Data.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class HomeController : BaseController
    {
        private readonly IProductService productService;
        private readonly ICategoryService categoryService;
        private readonly IUserStore<ApplicationUser> userStore;

        public HomeController(IProductService productServiceIn, ICategoryService categoryServiceIn, IUserStore<ApplicationUser> userStore)
        {
            this.productService = productServiceIn;
            this.categoryService = categoryServiceIn;
            this.userStore = userStore;
        }

        public IActionResult Index(int? active_category, int? active_subCategory, string toastr)
        {
            if (active_subCategory != null && active_category == null)
            {
                active_category = this.categoryService.GetCategoryFromSubCategory((int)active_subCategory);
            }
            else if (active_subCategory == null && active_category == null)
            {
                active_category = this.categoryService.GetInitialCategory();
                active_subCategory = this.categoryService.GetInitialSubCategory((int)active_category);
            }
            else if (active_category != null && active_subCategory == null)
            {
                active_subCategory = this.categoryService.GetInitialSubCategory((int)active_category);
            }

            this.ViewBag.SubCategories = this.categoryService.GetAllSubCategories((int)active_category);
            this.ViewBag.CurrentSubCategoryId = active_subCategory;
            this.ViewBag.CurrentCategoryId = active_category;
            this.ViewBag.Toastr = toastr;

            if (this.User.IsInRole(GlobalConstants.AdministratorRoleName))
            {
                var products = this.productService.GetAllAdminActiveProducts(active_subCategory != null ? (int)active_subCategory : -1);
                return this.View("Admin", products);
            }

            if (this.User.IsInRole(GlobalConstants.LocalAdministratorRoleName))
            {
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userLang = this.productService.GetCurrentUserLanguage(userId);
                var products = this.productService.GetAllLocalAdminActiveProducts(active_subCategory != null ? (int)active_subCategory : -1, userLang);
                return this.View("LocalAdmin", products);
            }

            return this.View();
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => this.View();
    }
}
