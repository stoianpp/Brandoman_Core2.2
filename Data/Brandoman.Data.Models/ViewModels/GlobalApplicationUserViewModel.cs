namespace Brandoman.Data.Models.ViewModels
{
    using System;

    using Brandoman.Data.Common.Models;
    using Brandoman.Services.Mapping;

    public class GlobalApplicationUserViewModel : IMapFrom<ApplicationUser>, IMapTo<ApplicationUser>
    {
        public string Id { get; set; }

        public Lang Lang { get; set; }

        public string UserName { get; set; }
    }
}
