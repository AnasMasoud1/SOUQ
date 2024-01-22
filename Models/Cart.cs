using System.ComponentModel.DataAnnotations.Schema;

namespace Souq.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public DateTime? DateTime { get; set; }
        public float? Price { get; set; }
        public float? TotalPrice { get; set; }
        public int? Quentity { get; set; }
        public string? UserID { get; set; }

        [ForeignKey("product")]
        public int? ProductId { get; set; }
        //public string Image { get; set; }
        public Product? product { get; set; }
    }
}
