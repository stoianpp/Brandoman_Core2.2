namespace Brandoman.Data.Models.ViewModels
{
    using Brandoman.Data.Common.Models;

    public class SubCategoryIndexViewModel
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public string LangText { get; set; }

        public int SubCategoryId { get; set; }

        public Lang Lang { get; set; }

        public byte[] Image { get; set; }
    }
}
