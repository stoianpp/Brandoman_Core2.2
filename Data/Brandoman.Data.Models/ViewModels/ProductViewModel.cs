namespace Brandoman.Data.Models.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using AutoMapper;
    using Brandoman.Services.Mapping;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class ProductViewModel : IMapFrom<Product>, IMapTo<Product>, IHaveCustomMappings
    {
        public int? Id { get; set; }

        [Display(Name = "Category")]
        public int SubCategoryId { get; set; }

        public string SubCategory { get; set; }

        [Required]
        [MinLength(3)]
        public string Name { get; set; }

        [Required]
        public string Details { get; set; }

        public byte[] Image { get; set; }

        public List<SelectListItem> SubCategories { get; set; }

        public int Order { get; set; }

        public bool Active { get; set; }

        public bool IsUpdate { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Product, ProductViewModel>()
                .ForMember(x => x.SubCategory, opt => opt.MapFrom(x => x.SubCategory.Name));
            configuration.CreateMap<ProductViewModel, Product>()
               .ForPath(x => x.SubCategory.Name, opt => opt.MapFrom(x => x.SubCategory));
        }
    }
}
