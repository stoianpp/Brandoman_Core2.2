namespace Brandoman.Data.Models
{
    using Brandoman.Data.Common.Models;

    public class Brand : BaseDeletableModel<int>
    {
        public string Name { get; set; }
    }
}
