namespace Brandoman.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

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
            this.ViewBag.Language = userLang;

            return this.View(users);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<JsonResult> DeleteUser(string id)
        {
            try
            {
                var user = this.userService.GetUserById(id);
                await this.userService.DeleteUser(user);
            }
            catch
            {
                return this.Json(this.Url.Action("Index", "User", new { toastr = "User hasn't been deleted. Try again." }));
            }

            return this.Json(this.Url.Action("Index", "User", new { toastr = "User has been successfully deleted." }));
        }
    }
}
