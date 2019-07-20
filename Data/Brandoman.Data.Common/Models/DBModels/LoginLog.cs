namespace Brandoman.Data.Common.Models.DBModels
{
    public class LoginLog : BaseDeletableModel<int>
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public Lang UserLang { get; set; }
    }
}
