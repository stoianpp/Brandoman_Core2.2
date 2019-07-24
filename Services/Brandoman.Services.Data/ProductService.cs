namespace Brandoman.Services.Data
{
    using System.Linq;

    using AutoMapper;
    using Brandoman.Data.Common.Repositories;
    using Brandoman.Data.Models;
    using Brandoman.Data.Models.ViewModels;
    using Brandoman.Services.Data.Interfaces;
    using Brandoman.Services.Mapping;

    public class ProductService : IProductService
    {
        private readonly IDeletableEntityRepository<Product> productRepository;
        private readonly IMapper mapper;

        public ProductService(IDeletableEntityRepository<Product> products, IMapper mapperIn)
        {
            this.productRepository = products;
            this.mapper = mapperIn;
        }

        public IQueryable<AdminIndexViewModel> GetAllAdminActiveProducts(int active_subCategory)
        {
            var products = this.productRepository.All().Where(x => x.SubCategoryId == active_subCategory);
            var viewModels = products.To<AdminIndexViewModel>();

            return viewModels;
        }

        public Product GetProductById(int id)
        {
            return this.productRepository.All().FirstOrDefault(x => x.Id == id);
        }
    }
}
