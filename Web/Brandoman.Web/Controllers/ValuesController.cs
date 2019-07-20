namespace Brandoman.Web.Controllers
{
    using Brandoman.Data;
    using Brandoman.Data.Common.Models;
    using Brandoman.Services;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ValuesController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly ILoginService loginService;

        public ValuesController(
            ApplicationDbContext context,
            ILoginService loginService)
        {
            this.context = context;
            this.loginService = loginService;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<string> Get()
        {
            return "123";
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

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
