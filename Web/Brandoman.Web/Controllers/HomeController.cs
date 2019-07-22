namespace Brandoman.Web.Controllers
{
    using Brandoman.Common;
    using Brandoman.Services.Data.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class HomeController : BaseController
    {
        private readonly IProductService productService;

        public HomeController(IProductService productServiceIn)
        {
            this.productService = productServiceIn;
        }

        public IActionResult Index()
        {
            if (this.User.IsInRole(GlobalConstants.AdministratorRoleName))
            {
                var products = this.productService.GetAllAdminActiveProducts();
                return this.View("Admin", products);
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
