namespace Brandoman.Data.Models.ViewModels
{
    using Brandoman.Services.Mapping;

    public class SubCategoryDropDownViewModel : IMapFrom<SubCategory>
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
