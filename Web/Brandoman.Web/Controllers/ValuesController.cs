namespace Brandoman.Web.Controllers
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;

    using Brandoman.Data;
    using Brandoman.Data.Common.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;

    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IOptions<JwtSettings> options;
        private readonly ApplicationDbContext context;

        public ValuesController(IOptions<JwtSettings> options, ApplicationDbContext context)
        {
            this.options = options;
            this.context = context;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<string> Get()
        {
            return this.options.Value.Secret;
        }

        // GET api/values
        [HttpGet("[action]")]
        public ActionResult<string> WhoAmI()
        {
            return "user: " + this.User.Identity.Name;
        }

        // GET api/values
        [HttpGet("[action]")]
        public ActionResult<string> Login(string username, string password)
        {
            var users = this.context.Users;
            var user = users.SingleOrDefault(x => x.UserName == username);
            if (user == null)
            {
                return null;
            }

            if (user != null)
            {
                var hasher = new PasswordHasher<IdentityUser>();
                if (hasher.VerifyHashedPassword(user, user.PasswordHash, password)
                    == PasswordVerificationResult.Failed)
                {
                    return null; // Return null if user not found
                }
            }

            var secret = this.options.Value.Secret;
            var key = Encoding.UTF8.GetBytes(secret);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, "admin"),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                                new SymmetricSecurityKey(key),
                                SecurityAlgorithms.HmacSha256Signature),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            return jwt;
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
