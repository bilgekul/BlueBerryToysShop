using System.ComponentModel.DataAnnotations;

namespace BlueBerry.ToysShop.Web.Models
{
    public class ListItem
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int ListId { get; set; }
        public ProductList List { get; set; }
    }
}
