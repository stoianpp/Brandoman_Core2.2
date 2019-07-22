namespace Brandoman.Data.Models
{
    using System.Collections.Generic;

    using Brandoman.Data.Common.Models;

    public class Product : BaseDeletableModel<int>
    {
        public Product()
        {
            this.ProductLanguages = new HashSet<ProductLang>();
        }

        public string Name { get; set; }

        public string Details { get; set; }

        public int SubCategoryId { get; set; }

        public virtual SubCategory SubCategory { get; set; }

        public int? Order { get; set; }

        public virtual ICollection<ProductLang> ProductLanguages { get; set; }

        public byte[] Image { get; set; }
    }
}
