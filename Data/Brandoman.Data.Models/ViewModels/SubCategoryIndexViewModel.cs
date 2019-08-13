namespace Brandoman.Data.Models.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    using Brandoman.Data.Common.Models;

    public class SubCategoryIndexViewModel
    {
        public int? Id { get; set; }

        [Required]
        [MinLength(3)]
        public string Name { get; set; }

        [Required]
        [MinLength(3)]
        [Display(Name = "Local Name")]
        public string LangText { get; set; }

        [Required]
        public int SubCategoryId { get; set; }

        [Required]
        public Lang Lang { get; set; }
    }
}
