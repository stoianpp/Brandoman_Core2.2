namespace Brandoman.Data.Models.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Brandoman.Data.Common.Models;
    using Brandoman.Data.Models;
    using Brandoman.Services.Mapping;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class AdminIndexViewModel : IMapFrom<Product>, IMapTo<Product>, IHaveCustomMappings
    {
        public int? Id { get; set; }

        [Display(Name = "Category")]
        public int SubCategoryId { get; set; }

        public string SubCategory { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Details { get; set; }

        public byte[] Image { get; set; }

        public List<SelectListItem> SubCategories { get; set; }

        public Lang Lang { get; set; }

        public int Order { get; set; }

        public bool Active { get; set; }

        public bool IsUpdate { get; set; }

        void IHaveCustomMappings.CreateMappings(AutoMapper.IProfileExpression configuration)
        {
            configuration.CreateMap<Product, AdminIndexViewModel>()
                .ForMember(x => x.SubCategory, opt => opt.MapFrom(x => x.SubCategory.Name));
        }
    }
}
