namespace Brandoman.Web.Controllers
{
    using System.Linq;

    using AutoMapper;
    using Brandoman.Common;
    using Brandoman.Data.Models.ViewModels;
    using Brandoman.Services.Data.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    [Authorize]

    public class ProductController : BaseController
    {
        private readonly ICategoryService categories;
        private readonly IMapper mapper;
        private readonly IProductService products;

        public ProductController(
            ICategoryService categoriesIn,
            IMapper mapperIn,
            IProductService productsIn)
        {
            this.categories = categoriesIn;
            this.mapper = mapperIn;
            this.products = productsIn;
        }

        [HttpGet]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public IActionResult AddEditRecord(int? id, int cat)
        {
            var subCategories = this.categories.GetAllSubCategories(cat);
            var product = new ProductViewModel();

            if (id != null)
            {
                this.ViewBag.Message = "Edit Product";
                product = this.mapper.Map<ProductViewModel>(this.products.GetProductById((int)id));

                product.IsUpdate = true;
                product.SubCategories = subCategories.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
                product.SubCategories.FirstOrDefault(x => int.Parse(x.Value) == product.SubCategoryId).Selected = true;
                return this.PartialView(product);
            }

            product.SubCategories = subCategories.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            product.IsUpdate = false;
            this.ViewBag.Message = "New Product";

            return this.PartialView(product);
        }
    }
}
