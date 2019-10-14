namespace Brandoman.Web.Areas.Administration.Controllers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Brandoman.Common;
    using Brandoman.Data.Common.Models;
    using Brandoman.Services;
    using Brandoman.Services.Data.Interfaces;
    using ClosedXML.Excel;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public class UserController : AdministrationController
    {
        private readonly IProductService productService;
        private readonly IUserService userService;
        private readonly ILoginService loginService;

        public UserController(IProductService productService, IUserService userService, ILoginService loginService)
        {
            this.productService = productService;
            this.userService = userService;
            this.loginService = loginService;
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

        [AllowAnonymous]
        public IActionResult GlobalIndex(string toastr)
        {
            var userId = this.GetUserId();
            var res = this.userService.GetRolesForCurrentUser(userId);
            var isAdministrator = res.Contains(GlobalConstants.AdministratorRoleName);

            if (isAdministrator)
            {
                var languages = Enum.GetValues(typeof(Lang)).Cast<Lang>();
                var users = this.userService.GetLocalAdminUsers();

                this.ViewBag.Title = "Local Administrators";
                this.ViewBag.Toastr = toastr;
                this.ViewBag.Languages = languages;

                return this.View(users);
            }

            return this.Unauthorized();
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

        [HttpGet]
        [Authorize(Roles = GlobalConstants.LocalAdministratorRoleName)]
        public IActionResult LogReport()
        {
            return this.View();
        }

        [Authorize(Roles = GlobalConstants.LocalAdministratorRoleName)]
        [HttpPost]
        public void GetLoginReport(string start, string end)
        {
            var userLang = this.productService.GetCurrentUserLanguage(this.GetUserId());
            var startDate = DateTime.Parse(start);
            var endDate = DateTime.Parse(end);

            var logsExtract = this.loginService.All().Where(x => x.UserLang == userLang && x.CreatedOn >= startDate && x.CreatedOn <= endDate).ToList();

            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Log Report");
                ws.Cell("A1").Value = "User Email";
                ws.Cell("B1").Value = "Login Time";
                for (int i = 0; i < logsExtract.Count; i++)
                {
                    string homeTeam = logsExtract[i].UserName;
                    string score = logsExtract[i].CreatedOn.ToString();
                    ws.Cell("A" + (i + 2)).Value = homeTeam;
                    ws.Cell("B" + (i + 2)).Value = score;
                }

                ws.Range("A1:B1").Style.Font.Bold = true;
                ws.Columns().AdjustToContents();

                MemoryStream fs = new MemoryStream();
                wb.SaveAs(fs);
                fs.Position = 0;
                string my_Name = WebUtility.UrlEncode("Log Report: " + DateTime.Now.ToShortDateString() + ".xlsx");
                MemoryStream stream = fs;

                this.Response.Clear();
                this.Response.Headers.Add("content-disposition", "attachment; filename=" + my_Name);
                this.Response.ContentType = "application/vnd.ms-excel";
                this.Response.Body.WriteAsync(stream.ToArray());
            }

            return;
        }
    }
}
