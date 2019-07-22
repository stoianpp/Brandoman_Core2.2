namespace Brandoman.Data.Models
{
    using System.Collections.Generic;

    using Brandoman.Data.Common.Models;

    public class SubCategory : BaseDeletableModel<int>
    {
        public SubCategory()
        {
            this.Products = new HashSet<Product>();
        }

        public string Name { get; set; }

        public int CategoryId { get; set; }

        public byte[] Image { get; set; }

        public virtual Category Category { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
