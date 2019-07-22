namespace Brandoman.Data.Models
{
    using Brandoman.Data.Common.Models;

    public class LoginLog : BaseDeletableModel<int>
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public Lang UserLang { get; set; }
    }
}
