namespace Brandoman.Data.Models.ViewModels
{
    using AutoMapper;
    using Brandoman.Services.Mapping;

    public class AppUserViewModel : IMapFrom<Product>, IMapTo<Product>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public int SubCategoryId { get; set; }

        public string SubCategory { get; set; }

        public string Name { get; set; }

        public byte[] Image { get; set; }

        public string LangText { get; set; }

        public string LangTitle { get; set; }

        public long? Timestamp { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Product, ProductViewModel>()
                .ForMember(x => x.SubCategory, opt => opt.MapFrom(x => x.SubCategory.Name));
        }
    }
}
