namespace Brandoman.Data.Models
{
    using System.Collections.Generic;

    using Brandoman.Data.Common.Models;

    public class Category : BaseDeletableModel<int>
    {
        public Category()
        {
            this.SubCategories = new HashSet<SubCategory>();
        }

        public string Name { get; set; }

        public byte[] Image { get; set; }

        public int BrandId { get; set; }

        public virtual Brand Brand { get; set; }

        public virtual ICollection<SubCategory> SubCategories { get; set; }
    }
}
