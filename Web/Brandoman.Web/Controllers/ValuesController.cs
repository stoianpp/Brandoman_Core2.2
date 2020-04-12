namespace Brandoman.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Security.Claims;

    using Brandoman.Common;
    using Brandoman.Data;
    using Brandoman.Data.Common.Models;
    using Brandoman.Data.Models;
    using Brandoman.Services;
    using Brandoman.Services.Data.Interfaces;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;

    [EnableCors("AllowOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ValuesController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly ILoginService loginService;
        private readonly IProductService productService;
        private readonly ICategoryService categoryService;
        private readonly ClaimsPrincipal caller;

        public ValuesController(
            ApplicationDbContext context,
            ILoginService loginService,
            IProductService productService,
            ICategoryService categoryService,
            ClaimsPrincipal caller)
        {
            this.context = context;
            this.loginService = loginService;
            this.productService = productService;
            this.categoryService = categoryService;
            this.caller = caller;
        }

        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            var claims = this.caller.Claims.Select(c => new { c.Type, c.Value });
            var userId = claims.FirstOrDefault(x => x.Type == "iss").Value;
            var userLang = this.productService.GetCurrentUserLanguage(userId);
            var userName = claims.FirstOrDefault(x => x.Type == GlobalConstants.NameClaim).Value;

            var newLogin = new LoginLog { UserId = userId, UserName = userName, UserLang = userLang };
            this.loginService.LoginRecord(newLogin);

            var data = this.productService.GetAppData(userLang);

            Microsoft.Extensions.Primitives.StringValues timestamp;
            try
            {
                var result = this.Request.Headers.TryGetValue("timestamp", out timestamp);
            }
            catch
            {
                return this.BadRequest();
            }

            var lastUpdated = data.First().Timestamp;
            var splitInput = timestamp.ToString().Split(new char[] { ' ' });
            var longTimestampNum = long.Parse(splitInput[0]);
            if (lastUpdated <= longTimestampNum && (splitInput.Length > 1 && ((Lang)Enum.Parse(typeof(Lang), splitInput[1]) == userLang)))
            {
                return this.NotFound();
            }

            var subCatIds = data.Select(x => x.SubCategoryId).Distinct();
            var subCatsFull = this.categoryService.GetAllSubCategories().Where(x => subCatIds.Contains(x.Id)).ToList();
            var localSubCats = this.categoryService.LocalizeSubCats(subCatsFull, userLang);
            var subCats = localSubCats.Select(x => new { x.Name, x.Image, x.CategoryId, x.Id }).ToList();
            var catsAll = this.categoryService.GetAllFullCategories();

            var cats = (from s in subCatsFull
                        from c in catsAll
                        where c.SubCategories.Contains(s)
                        select new { c.Name, c.Image, c.Id }).Distinct();
            var longTimestamp = longTimestampNum.ToString() + " " + userLang.ToString();
            return new JsonResult(new { data, cats, subCats, lastUpdated, longTimestamp });
        }

        // GET api/values/login
        [AllowAnonymous]
        [HttpGet("[action]")]
        public ActionResult<string> Login(string username, string password)
        {
            var result = this.loginService.GetToken(this.context.Users, username, password);
            if (result == null)
            {
                return this.BadRequest("Could not create token");
            }

            return this.Ok(new { token = result });
        }
    }
}
