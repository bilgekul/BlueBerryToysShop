using System.ComponentModel.DataAnnotations.Schema;

namespace BlueBerry.ToysShop.Web.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Brand { get; set; }   
        public decimal Price { get; set; } 
        public int Quantity { get; set; }
        public int Rating { get; set; }
        public int Expire { get; set; }
        public string? ImagePath { get; set; }
        public DateTime? PublishDate { get; set; }

        [ForeignKey("CategoryId")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

    }
}
