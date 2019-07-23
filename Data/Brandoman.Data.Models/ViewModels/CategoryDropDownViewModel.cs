namespace Brandoman.Data.Models.ViewModels
{
    using Brandoman.Services.Mapping;

    public class CategoryDropDownViewModel : IMapFrom<Category>
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
