namespace Brandoman.Web.Controllers
{
    using System.Linq;
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

            active_subCategory = active_subCategory == null ? 0 : active_subCategory;

            this.ViewBag.SubCategories = this.categoryService.GetAllSubCategories((int)active_category);
            this.ViewBag.CurrentSubCategoryId = active_subCategory;
            this.ViewBag.CurrentCategoryId = active_category;
            this.ViewBag.Toastr = toastr;

            if (this.User.IsInRole(GlobalConstants.AdministratorRoleName))
            {
                var products = this.productService.GetAllAdminActiveProducts(active_subCategory != null ? (int)active_subCategory : -1);
                return this.View("Admin", products);
            }

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userLang = this.productService.GetCurrentUserLanguage(userId);
            if (this.User.IsInRole(GlobalConstants.LocalAdministratorRoleName))
            {
                var products = this.productService.GetAllLocalAdminActiveProducts(active_subCategory != null ? (int)active_subCategory : -1, userLang);
                return this.View("LocalAdmin", products);
            }

            var data = this.productService.GetEndUserIndexData((int)active_subCategory, userLang).ToList();
            this.ViewBag.Title = data.Count > 0 ? "Product List" : "No Data";
            return this.View(data);
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => this.View();

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public ActionResult UpdateLanguage(string lang)
        {
            this.productService.UpdateLanguage(lang);
            return this.Redirect("/");
        }
    }
}
