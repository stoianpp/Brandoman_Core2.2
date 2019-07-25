namespace Brandoman.Web.Controllers
{
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper;
    using Brandoman.Common;
    using Brandoman.Data.Models;
    using Brandoman.Data.Models.ViewModels;
    using Brandoman.Services.Data.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
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
                var currentProduct = this.products.GetProductById((int)id);
                product.Name = currentProduct.Name;
                product.Details = currentProduct.Details;
                product.SubCategoryId = currentProduct.SubCategoryId;

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

        [HttpPost]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public ActionResult AddEditRecord(ProductViewModel productVM, IFormFile imageName)
        {
            if (this.ModelState.IsValid)
            {
                if (productVM.Id == null)
                {
                    Product product = new Product { Name = productVM.Name, Details = productVM.Details, SubCategoryId = productVM.SubCategoryId };
                    try
                    {
                        this.products.SaveProductAsync(product, imageName);
                        return this.RedirectToAction("Index", "Home", new { active_subCategory = productVM.SubCategoryId });
                    }
                    catch
                    {
                    }
                }
                else
                {
                    try
                    {
                        var product = this.products.GetProductById((int)productVM.Id);
                        product.Name = productVM.Name;
                        product.Details = productVM.Details;
                        product.SubCategoryId = productVM.SubCategoryId;
                        this.products.SaveProductAsync(product, imageName);

                        // Code for pictures uploading of SubCategories through product edit window
                        // var cat1 = _subCategories.GetAll().Where(x => x.Id == 4).FirstOrDefault();
                        // cat1.Image = binData;
                        // _subCategories.Update(cat1);
                    }
                    catch
                    {
                    }

                    return this.RedirectToAction("Index", "Home", new { active_subCategory = productVM.SubCategoryId });
                }
            }

            var cat = this.categories.GetCategoryFromSubCategory(productVM.SubCategoryId);
            productVM.SubCategories = this.categories.GetAllSubCategories(cat).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            this.ViewBag.Message = productVM.Id != null ? "Edit Product" : "New Product";
            return this.View(productVM);
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<ActionResult> DeleteRecord(int id)
        {
            bool result;
            try
            {
                var product = this.products.GetProductById(id);
                result = await this.products.Delete(product);
            }
            catch
            {
                result = false;
            }

            return this.Json(new { success = result });
        }
    }
}
