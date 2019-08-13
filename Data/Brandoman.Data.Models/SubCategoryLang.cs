namespace Brandoman.Data.Models
{
    using Brandoman.Data.Common.Models;

    public class SubCategoryLang : BaseDeletableModel<int>
    {
        public Lang Lang { get; set; }

        public int SubCategoryId { get; set; }

        public virtual SubCategory SubCategory { get; set; }

        public string Name { get; set; }

        public string LangText { get; set; }
    }
}
