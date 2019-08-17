namespace Brandoman.Services.Data.Interfaces
{
    using System.Linq;
    using System.Threading.Tasks;

    using Brandoman.Data.Common.Models;
    using Brandoman.Data.Common.Repositories;
    using Brandoman.Data.Models;
    using Brandoman.Data.Models.ViewModels;
    using Brandoman.Services.Mapping;

    public class UserService : IUserService
    {
        private readonly IDeletableEntityRepository<ApplicationUser> userRepository;

        public UserService(IDeletableEntityRepository<ApplicationUser> userRepository)
        {
            this.userRepository = userRepository;
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
    }
}
