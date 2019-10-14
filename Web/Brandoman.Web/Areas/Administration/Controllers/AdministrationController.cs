namespace Brandoman.Web.Areas.Administration.Controllers
{
    using System.Security.Claims;

    using Brandoman.Common;
    using Brandoman.Data.Common.Models;
    using Brandoman.Web.Controllers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Roles = GlobalConstants.LocalAdministratorRoleName + "," + GlobalConstants.AdministratorRoleName)]
    [Area("Administration")]
    public class AdministrationController : BaseController
    {
        internal string GetUserId()
        {
            return this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
