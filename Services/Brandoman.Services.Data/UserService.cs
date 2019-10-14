namespace Brandoman.Services.Data.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Brandoman.Common;
    using Brandoman.Data.Common.Models;
    using Brandoman.Data.Common.Repositories;
    using Brandoman.Data.Models;
    using Brandoman.Data.Models.ViewModels;
    using Brandoman.Services.Mapping;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;

    public class UserService : IUserService
    {
        private readonly IDeletableEntityRepository<ApplicationUser> userRepository;
        private readonly IServiceProvider serviceProvider;

        public UserService(IDeletableEntityRepository<ApplicationUser> userRepository, IServiceProvider serviceProvider)
        {
            this.userRepository = userRepository;
            this.serviceProvider = serviceProvider;
        }

        public IQueryable<ApplicationUserIndexViewModel> GetUsersByLanguage(Lang lang)
        {
            var result = this.userRepository.All().Where(x => x.Lang == lang).OrderBy(x => x.UserName);
            return result.To<ApplicationUserIndexViewModel>();
        }

        public ApplicationUser GetUserById(string user)
        {
            var result = this.userRepository.All().FirstOrDefault(x => x.Id == user);
            return result;
        }

        public async Task<bool> DeleteUser(ApplicationUser user)
        {
            var result = true;
            try
            {
                this.userRepository.Delete(user);
                await this.userRepository.SaveChangesAsync();
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public int GetUsersNumber()
        {
            return this.userRepository.All().Count();
        }

        public IQueryable<GlobalApplicationUserViewModel> GetLocalAdminUsers()
        {
            var roleManager = this.serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var role = roleManager.FindByNameAsync(GlobalConstants.LocalAdministratorRoleName).Result.Id;
            var result = this.userRepository.All().Where(x => x.Roles.Any(y => y.RoleId == role));
            return result.To<GlobalApplicationUserViewModel>();
        }

        public IList<string> GetRolesForCurrentUser(string userId)
        {
            var userManager = this.serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var currentUser = userManager.FindByIdAsync(userId).Result;
            var roles = userManager.GetRolesAsync(currentUser).Result.ToList();

            return roles;
        }
    }
}
