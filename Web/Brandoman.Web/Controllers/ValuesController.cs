namespace Brandoman.Web.Controllers
{
    using System.Linq;
    using System.Security.Claims;

    using Brandoman.Data;
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
            var longTimestamp = long.Parse(timestamp);
            if (lastUpdated <= longTimestamp)
            {
                return this.NotFound();
            }

            var subCatIds = data.Select(x => x.SubCategoryId).Distinct();
            var subCatsFull = this.categoryService.GetAllSubCategories().Where(x => subCatIds.Contains(x.Id)).ToList();

            // Add local names for subcategories
            // foreach (var item in subCatsFull)
            // {
            //     var newName = LocalResource.Resource.ResourceManager.GetString(item.Name.Replace(" ", "_"));
            //     item.Name = newName ?? item.Name;
            // }
            var subCats = subCatsFull.Select(x => new { x.Name, x.Image, x.CategoryId, x.Id }).ToList();
            var catsAll = this.categoryService.GetAllFullCategories();

            var cats = (from s in subCatsFull
                        from c in catsAll
                        where c.SubCategories.Contains(s)
                        select new { c.Name, c.Image, c.Id }).Distinct();

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
