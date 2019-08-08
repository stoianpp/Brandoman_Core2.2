namespace Brandoman.Web.Controllers
{
    using System.Linq;
    using System.Security.Claims;

    using Brandoman.Data;
    using Brandoman.Services;
    using Brandoman.Services.Data.Interfaces;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ValuesController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly ILoginService loginService;
        private readonly IProductService productService;
        private readonly ClaimsPrincipal caller;

        public ValuesController(
            ApplicationDbContext context,
            ILoginService loginService,
            IProductService productService,
            ClaimsPrincipal caller)
        {
            this.context = context;
            this.loginService = loginService;
            this.productService = productService;
            this.caller = caller;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<string> Get()
        {
            var claims = this.caller.Claims.Select(c => new { c.Type, c.Value });
            var userId = claims.FirstOrDefault(x => x.Type == "iss").Value;
            var userLang = this.productService.GetCurrentUserLanguage(userId);

            return " ";
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
