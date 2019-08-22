namespace Brandoman.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper;
    using Brandoman.Common;
    using Brandoman.Data.Common.Models;
    using Brandoman.Data.Models;
    using Brandoman.Data.Models.ViewModels;
    using Brandoman.Services.Data.Interfaces;
    using Ganss.XSS;
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
        private readonly IHtmlSanitizer sanitizer;

        public ProductController(
            ICategoryService categoriesIn,
            IMapper mapperIn,
            IProductService productsIn,
            IHtmlSanitizer sanitizer)
        {
            this.categories = categoriesIn;
            this.mapper = mapperIn;
            this.products = productsIn;
            this.sanitizer = sanitizer;
        }

        [Route("Product/AddEditRecord")]
        [HttpGet("{cat,subCat,id}")]
        public IActionResult AddEditRecord(int cat, int subCat, int? id)
        {
            var subCategories = this.categories.GetAllSubCategories(cat);
            var product = new ProductViewModel();

            if (id != null)
            {
                this.ViewBag.Message = "Edit Product";
                var currentProduct = this.products.GetProductById((int)id);
                product.Name = this.sanitizer.Sanitize(currentProduct.Name);
                product.Details = currentProduct.Details;
                product.SubCategoryId = subCat;
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

        [HttpPost]
        [IgnoreAntiforgeryToken]
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

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public ActionResult SaveProductOrder([FromBody]IEnumerable<OrderViewModel> orders)
        {
            int subCategoryId = this.products.GetProductById(orders.FirstOrDefault().Id).SubCategoryId;
            try
            {
                this.products.MultipleUpdate(orders);
            }
            catch
            {
                return this.Json(this.Url.Action("Index", "Home", new { active_subCategory = subCategoryId, toastr = "New order hasn't been recorded. Try again." }));
            }

            return this.Json(this.Url.Action("Index", "Home", new { active_subCategory = subCategoryId, toastr = "New order have been successfully recorder." }));
        }

        [Route("Product/AddEditTranslation")]
        [HttpGet("{cat,productId}")]
        public IActionResult AddEditTranslation(int cat, int productId, int? id, Lang lang)
        {
            var product = this.products.GetProductById(productId);
            var subCategory = this.categories.GetSubCategoryName(cat);

            var translation = this.products.GetNewTranslation(cat, productId, id, lang, product, subCategory);

            if (id != null)
            {
                this.ViewBag.Message = "Edit Translation";
                return this.View(translation);
            }

            this.ViewBag.Message = "New Translation";
            return this.View(translation);
        }

        [ActionName("AddEditTranslations")]
        [HttpPost]
        [Authorize(Roles = GlobalConstants.LocalAdministratorRoleName)]
        public IActionResult AddEditTranslations(TranslationViewModel translationIn)
        {
            if (this.ModelState.IsValid)
            {
                if (translationIn.Id == null)
                {
                    try
                    {
                        this.products.SaveTranslationAsync(translationIn);
                        return this.RedirectToAction("Index", "Home", new { active_subCategory = translationIn.SubCategoryId, toastr = "New translation has been created successfully." });
                    }
                    catch
                    {
                        return this.RedirectToAction("Index", "Home", new { active_subCategory = translationIn.SubCategoryId, toastr = "ERROR: No translation has been created. Try again." });
                    }
                }
                else
                {
                    try
                    {
                        this.products.UpdateTranslation(translationIn);
                        return this.RedirectToAction("Index", "Home", new { active_subCategory = translationIn.SubCategoryId, toastr = "The translation has been updated successfully" });
                    }
                    catch
                    {
                        return this.RedirectToAction("Index", "Home", new { active_subCategory = translationIn.SubCategoryId, toastr = "ERROR: Translation hasn't been updated. Try again." });
                    }
                }
            }

            this.ViewBag.Message = translationIn.Id != null ? "Edit Translation" : "New Translation";
            return this.View(translationIn);
        }
    }
}
