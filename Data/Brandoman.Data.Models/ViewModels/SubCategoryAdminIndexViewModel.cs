namespace Brandoman.Data.Models.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class SubCategoryAdminIndexViewModel
    {
        public int? Id { get; set; }

        [Required]
        [MinLength(3)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public string Category { get; set; }

        public byte[] Image { get; set; }
    }
}
