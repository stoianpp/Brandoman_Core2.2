namespace Brandoman.Services
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    using Brandoman.Data.Common.Models;
    using Brandoman.Data.Common.Repositories;
    using Brandoman.Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;

    using PasswordVerificationResult = Microsoft.AspNetCore.Identity.PasswordVerificationResult;

    public class LoginService : ILoginService
    {
        private readonly IOptions<JwtSettings> options;
        private readonly IDeletableEntityRepository<LoginLog> loginRepository;

        public LoginService(IOptions<JwtSettings> options, IDeletableEntityRepository<LoginLog> loginRepository)
        {
            this.options = options;
            this.loginRepository = loginRepository;
        }

        public string GetToken(IEnumerable<IdentityUser> users, string username, string password)
        {
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
                    return null;
                }
            }

            var secret = this.options.Value.Secret;
            var key = Encoding.UTF8.GetBytes(secret);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                }),
                Issuer = user.Id,
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                                new SymmetricSecurityKey(key),
                                SecurityAlgorithms.HmacSha256Signature),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            return jwt;
        }

        public async Task LoginRecord(LoginLog login)
        {
            await this.loginRepository.AddAsync(login);
            this.loginRepository.SaveChanges();
        }
    }
}
