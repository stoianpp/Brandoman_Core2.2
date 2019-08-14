namespace Brandoman.Web.Areas.Administration.Controllers
{
    using Brandoman.Common;
    using Brandoman.Services.Data.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class UserController : AdministrationController
    {
        private readonly IProductService productService;
        private readonly IUserService userService;

        public UserController(IProductService productService, IUserService userService)
        {
            this.productService = productService;
            this.userService = userService;
        }

        [Authorize(Roles = GlobalConstants.LocalAdministratorRoleName)]
        public IActionResult Index(string toastr)
        {
            var userLang = this.productService.GetCurrentUserLanguage(this.GetUserId());
            var users = this.userService.GetUsersByLanguage(userLang);

            this.ViewBag.Title = "Local Users Administration";
            this.ViewBag.Toastr = toastr;

            return this.View(users);
        }
    }
}
