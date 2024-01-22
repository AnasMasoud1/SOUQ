using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Souq.Models
{
    public class Product
    {
        public int Id { get; set; }
        [DisplayName("Product Name : ")]
        public string? Name { get; set; }
        public string? Description { get; set; }
        public float? Price { get; set;}
        public DateTime? CreatedDate { get; set; }
        public string? Image { get; set; }
        [DisplayName("User Name : ")]
        public string? UserID { get; set; }
        [NotMapped]
        [DisplayName ("Upload Image")]
        public IFormFile? ImageFile { get; set; }

        [ForeignKey ("category")]
        public int? CategoryId { get; set; }
        public Category? category { get; set; }
    }
}
