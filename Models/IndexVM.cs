namespace Souq.Models
{
    public class IndexVM
    {
        public IndexVM() {
            Categories=new List<Category>();
            Products=new List<Product>();
            Reviews=new List<Review>();
        }
        public List<Category> Categories { get; set; }
        public List<Product> Products { get; set; }
        public List<Review> Reviews { get; set; }

    }
}
