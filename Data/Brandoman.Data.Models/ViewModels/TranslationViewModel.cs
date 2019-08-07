namespace Brandoman.Data.Models.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    using Brandoman.Data.Common.Models;

    public class TranslationViewModel
    {
        public int? Id { get; set; }

        public string Text { get; set; }

        public string Translation { get; set; }

        [Required]
        public bool Active { get; set; }

        [Required]
        [MinLength(3)]
        public string TitleTranslation { get; set; }

        public string Title { get; set; }

        public Lang Lang { get; set; }

        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        public bool IsUpdate { get; set; }

        public string SubCategory { get; set; }

        public int SubCategoryId { get; set; }
    }
}
