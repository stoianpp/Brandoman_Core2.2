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

        [Route("Product/AddEditRecord")]
        [HttpGet("{cat,id}")]
        public IActionResult AddEditRecord(int cat, int? id)
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
                product.Image = currentProduct.Image;

                product.IsUpdate = true;
                product.SubCategories = subCategories.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
                product.SubCategories.FirstOrDefault(x => int.Parse(x.Value) == product.SubCategoryId).Selected = true;
                return this.View(product);
            }

            product.SubCategories = subCategories.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            product.IsUpdate = false;
            this.ViewBag.Message = "New Product";

            return this.View(product);
        }

        [ActionName("AddEditRecords")]
        [HttpPost]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public IActionResult AddEditRecords(ProductViewModel productVM, IFormFile imageName)
        {
            if (this.ModelState.IsValid)
            {
                if (productVM.Id == null)
                {
                    Product product = new Product { Name = productVM.Name, Details = productVM.Details, SubCategoryId = productVM.SubCategoryId };
                    try
                    {
                        this.products.SaveProductAsync(product, imageName);
                        return this.RedirectToAction("Index", "Home", new { active_subCategory = productVM.SubCategoryId, toastr = "New record has been created successfully." });
                    }
                    catch
                    {
                        return this.RedirectToAction("Index", "Home", new { active_subCategory = productVM.SubCategoryId, toastr = "ERROR: No record has been created. Try again." });
                    }
                }
                else
                {
                    try
                    {
                        this.products.Update(productVM, imageName);
                        return this.RedirectToAction("Index", "Home", new { active_subCategory = productVM.SubCategoryId, toastr = "The record has been updated successfully" });

                        // Code for pictures uploading of SubCategories through product edit window
                        // var cat1 = _subCategories.GetAll().Where(x => x.Id == 4).FirstOrDefault();
                        // cat1.Image = binData;
                        // _subCategories.Update(cat1);
                    }
                    catch
                    {
                        return this.RedirectToAction("Index", "Home", new { active_subCategory = productVM.SubCategoryId, toastr = "ERROR: Record hasn't been updated. Try again." });
                    }
                }
            }

            var cat = this.categories.GetCategoryFromSubCategory(productVM.SubCategoryId);
            productVM.SubCategories = this.categories.GetAllSubCategories(cat).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            this.ViewBag.Message = productVM.Id != null ? "Edit Product" : "New Product";
            return this.View(productVM);
        }

        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<JsonResult> DeleteRecord(int id)
        {
            string subCategory = null;
            try
            {
                var product = this.products.GetProductById(id);
                subCategory = product.SubCategoryId.ToString();
                await this.products.Delete(product);
            }
            catch
            {
                return this.Json(this.Url.Action("Index", "Home", new { active_subCategory = subCategory, toastr = "Record hasn't been deleted. Try again." }));
            }

            return this.Json(this.Url.Action("Index", "Home", new { active_subCategory = subCategory, toastr = "Record was successfully deleted." }));
        }
    }
}
