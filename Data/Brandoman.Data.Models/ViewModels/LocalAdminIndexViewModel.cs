namespace Brandoman.Data.Models.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    using AutoMapper;
    using Brandoman.Data.Common.Models;
    using Brandoman.Services.Mapping;

    public class LocalAdminIndexViewModel : IMapFrom<Product>, IHaveCustomMappings
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        [Display(Name = "Category")]
        public int? SubCategoryId { get; set; }

        public string SubCategory { get; set; }

        public string LangText { get; set; }

        public string Details { get; set; }

        public bool Active { get; set; }

        public string Title { get; set; }

        public Lang Lang { get; set; }

        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Product, LocalAdminIndexViewModel>()
                .ForMember(x => x.SubCategory, opt => opt.MapFrom(x => x.SubCategory.Name))
                .ForMember(x => x.ProductId, opt => opt.MapFrom(x => x.Id))
                .ForMember(x => x.Id, opt => opt.Ignore());
        }
    }
}
