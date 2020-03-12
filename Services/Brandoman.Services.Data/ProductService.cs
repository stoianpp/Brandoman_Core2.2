namespace Brandoman.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    using Brandoman.Data.Common.Models;
    using Brandoman.Data.Common.Repositories;
    using Brandoman.Data.Models;
    using Brandoman.Data.Models.ViewModels;
    using Brandoman.Services.Data.Interfaces;
    using Brandoman.Services.Mapping;
    using Ganss.XSS;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;

    public class ProductService : IProductService
    {
        private readonly IDeletableEntityRepository<Product> productRepository;
        private readonly IDeletableEntityRepository<ProductLang> translationRepository;
        private readonly IUserStore<ApplicationUser> userStore;
        private readonly IHtmlSanitizer sanitizer;

        public ProductService(
            IDeletableEntityRepository<Product> products,
            IDeletableEntityRepository<ProductLang> translations,
            IUserStore<ApplicationUser> userStore,
            IHtmlSanitizer sanitizer)
        {
            this.productRepository = products;
            this.translationRepository = translations;
            this.userStore = userStore;
            this.sanitizer = sanitizer;
        }

        public IQueryable<Product> GetAll()
        {
            return this.productRepository.All();
        }

        public IQueryable<AdminIndexViewModel> GetAllAdminActiveProducts(int active_subCategory)
        {
            var products = this.productRepository.All().Where(x => x.SubCategoryId == active_subCategory).OrderBy(x => x.Order);
            var viewModels = products.To<AdminIndexViewModel>();
            foreach (var item in viewModels)
            {
                item.Details = this.sanitizer.Sanitize(item.Details);
                item.Name = this.sanitizer.Sanitize(item.Name);
            }

            return viewModels;
        }

        public IQueryable<ProductLang> GetAllTranslations()
        {
            return this.translationRepository.All();
        }

        public IQueryable<LocalAdminIndexViewModel> GetAllLocalAdminActiveProducts(int active_subCategory, Lang language)
        {
            var allProductLangs = this.translationRepository.All().Where(x => x.Lang == language);
            var translationIds = allProductLangs.Select(x => x.ProductId).ToList();
            var products = this.productRepository.All().Where(x => x.SubCategoryId == active_subCategory);
            var adminProductViewModels = products.To<LocalAdminIndexViewModel>().ToList();
            foreach (var product in adminProductViewModels.Where(x => translationIds.Contains((int)x.ProductId)))
            {
                var translation = allProductLangs.Where(x => x.ProductId == product.ProductId).FirstOrDefault();
                product.LangText = translation.Text;
                product.Id = translation.Id;
                product.Active = translation.Active;
                product.Title = translation.Title ?? product.Name;
                product.LangTextLastUpdate = translation.ModifiedOn ?? translation.CreatedOn;
            }

            foreach (var product in adminProductViewModels)
            {
                product.Lang = language;
            }

            return adminProductViewModels.AsQueryable();
        }

        public IQueryable<EndUserViewModel> GetEndUserIndexData(int cat, Lang lang)
        {
            var allProductLangs = this.translationRepository.All().Where(x => x.Lang == lang && x.Active == true);
            var allProducts = this.productRepository.All().Where(x => x.SubCategoryId == cat);
            var products = allProducts.Select(x => x.Id).ToList();
            var result = allProductLangs.Where(x => products.Contains(x.ProductId)).Select(x =>
                                                new EndUserViewModel
                                                {
                                                    Title = x.Title,
                                                    Text = x.Text,
                                                    Image = allProducts.FirstOrDefault(y => y.Id == x.ProductId).Image,
                                                });
            foreach (var item in result)
            {
                item.Text = this.sanitizer.Sanitize(item.Text);
                item.Title = this.sanitizer.Sanitize(item.Title);
            }

            return result;
        }

        public Product GetProductById(int id)
        {
            return this.productRepository.All().FirstOrDefault(x => x.Id == id);
        }

        public async Task SaveProductAsync(Product product, IFormFile imageName)
        {
            if (imageName != null && imageName.Length > 0)
            {
                BinaryReader b = new BinaryReader(imageName.OpenReadStream());
                byte[] binData = b.ReadBytes((int)imageName.Length);
                product.Image = binData;
            }
            else
            {
                string file = Directory.GetCurrentDirectory() + @"\wwwroot\Images\missing.jpg";
                byte[] image = File.ReadAllBytes(file);
                product.Image = image;
            }

            await this.productRepository.AddAsync(product);
            this.productRepository.SaveChanges();
        }

        public async void UpdateLanguage(string lang)
        {
            var uri = new Uri("http://backend.wilkinson-sword.com.pl/data/products");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            string result = string.Empty;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();

                dynamic json = JsonConvert.DeserializeObject(result);

                Lang currentLang = Lang.Slovakian;
                switch (lang)
                {
                    case "en": currentLang = Lang.English; break;
                    case "bg": currentLang = Lang.Bulgarian; break;
                    case "rs": currentLang = Lang.Serbian; break;
                    case "ro": currentLang = Lang.Romanian; break;
                    case "pl": currentLang = Lang.Polish; break;
                    case "al": currentLang = Lang.Albanian; break;
                    case "kv": currentLang = Lang.Kosovo; break;
                    case "mk": currentLang = Lang.Macedonian; break;
                }

                foreach (var item in json[lang].products)
                {
                    string curruntProduct = item.ToString();
                    var record = JsonConvert.DeserializeObject<ProductFromDatabaseViewModel>(curruntProduct);
                    try
                    {
                        var product = this.GetAll().FirstOrDefault(x => x.Name == record.name && x.IsDeleted == false).Id;
                        var translation = this.GetAllTranslations().FirstOrDefault(x => x.Title == record.name && x.ProductId == product && x.Lang == currentLang);

                        if (translation == null)
                        {
                            var transl = new TranslationViewModel
                            {
                                ProductId = product,
                                Translation = record.content + "<br>" + record.characteristics[0],
                                Lang = currentLang,
                                TitleTranslation = record.name,
                            };

                            await this.SaveTranslationAsync(transl);
                        }
                        else
                        {
                            var transl = new TranslationViewModel
                            {
                                Id = translation.Id,
                                Translation = record.content + "<br>" + record.characteristics[0],
                                ProductId = product,
                                Lang = currentLang,
                                TitleTranslation = record.name,
                            };
                            translation.Text = record.content + "<br>" + record.characteristics[0];
                            this.UpdateTranslation(transl);
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        public async Task SaveTranslationAsync(TranslationViewModel translationIn)
        {
            ProductLang productTranslation = new ProductLang
            {
                Title = translationIn.TitleTranslation,
                Text = translationIn.Translation,
                Active = translationIn.Active,
                Lang = translationIn.Lang,
                ProductId = translationIn.ProductId,
            };

            await this.translationRepository.AddAsync(productTranslation);
            this.productRepository.SaveChanges();
        }

        public void UpdateAsync(ProductViewModel productVM, IFormFile imageName)
        {
        }

        public bool Update(ProductViewModel productVM, IFormFile imageName)
        {
            bool result;
            try
            {
                var product = this.productRepository.All().FirstOrDefault(x => x.Id == productVM.Id);
                product.Name = productVM.Name;
                product.Details = productVM.Details;
                product.SubCategoryId = productVM.SubCategoryId;

                if (imageName != null && imageName.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        imageName.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        product.Image = fileBytes;
                    }
                }

                this.productRepository.Update(product);
                this.productRepository.SaveChanges();
                result = true;
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public bool UpdateTranslation(TranslationViewModel translationIn)
        {
            bool result;
            try
            {
                var translation = this.translationRepository.All().FirstOrDefault(x => x.Id == translationIn.Id);
                translation.Title = translationIn.TitleTranslation;
                translation.Text = translationIn.Translation;
                translation.Active = translationIn.Active;
                this.translationRepository.Update(translation);
                this.translationRepository.SaveChanges();
                result = true;
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public async Task<bool> Delete(Product toDelete)
        {
            var result = true;
            try
            {
                this.productRepository.Delete(toDelete);
                await this.productRepository.SaveChangesAsync();
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public void MultipleUpdate(IEnumerable<OrderViewModel> orders)
        {
            var productIds = orders.Select(x => x.Id);
            var productList = this.productRepository.All().Where(x => productIds.Contains(x.Id)).ToList();
            foreach (var product in productList)
            {
                product.Order = orders.First(x => x.Id == product.Id).Order;
                this.productRepository.Update(product);
                this.productRepository.SaveChanges();
            }
        }

        public Lang GetCurrentUserLanguage(string userId)
        {
            var c_Token = CancellationToken.None;
            var language = this.userStore.FindByIdAsync(userId, c_Token).Result.Lang;
            return language;
        }

        public TranslationViewModel GetTranslationById(int id)
        {
            var translation = this.translationRepository.All().FirstOrDefault(x => x.Id == id);
            var result = new TranslationViewModel
            {
                Id = translation.Id,
                Translation = translation.Text,
                TitleTranslation = translation.Title,
                Active = translation.Active,
            };

            return result;
        }

        public TranslationViewModel GetNewTranslation(int cat, int productId, int? id, Lang lang, Product product, string subCategory)
        {
            var translation = new TranslationViewModel();

            if (id != null)
            {
                translation = this.GetTranslationById((int)id);
                translation.IsUpdate = true;
                translation.Text = product.Details;
                translation.Title = product.Name;
                translation.Lang = lang;
                translation.SubCategory = subCategory;
                translation.SubCategoryId = cat;
                translation.Product = product;
                return translation;
            }

            translation.IsUpdate = false;
            translation.Product = product;
            translation.Title = this.sanitizer.Sanitize(product.Name);
            translation.TitleTranslation = this.sanitizer.Sanitize(product.Name);
            translation.Text = product.Details;
            translation.Lang = lang;
            translation.SubCategory = subCategory;
            translation.SubCategoryId = cat;
            return translation;
        }

        public IEnumerable<AppUserViewModel> GetAppData(Lang lang)
        {
            var translations = this.translationRepository.All().Where(x => x.Lang == lang && x.Active == true).ToList();

            var lastEdited = (DateTime)translations.Max(x => x.ModifiedOn);
            var lastCreated = translations.Max(x => x.CreatedOn);
            var lastUpdated = DateTime.Compare(lastEdited, lastCreated) < 0 ? lastCreated : lastEdited;

            var products = this.productRepository.All().Include(x => x.SubCategory).ToList();

            var prod = from p in products
                       from l in translations
                       where p.ProductLanguages.Contains(l)
                       select p;

            IEnumerable<AppUserViewModel> trans = prod.Select(x => new AppUserViewModel
            {
                Name = translations.Where(y => y.Lang == lang && y.ProductId == x.Id)
                                    .FirstOrDefault()
                                    .Title ?? x.Name,
                Image = x.Image,
                Id = x.Id,
                SubCategoryId = x.SubCategoryId,
                SubCategory = x.SubCategory.Name,
                Timestamp = ((DateTimeOffset)lastUpdated).ToUnixTimeSeconds(),
                LangText = translations.Where(y => y.Lang == lang && y.ProductId == x.Id)
                                    .FirstOrDefault()
                                    .Text,
            });

            return trans;
        }
    }
}
