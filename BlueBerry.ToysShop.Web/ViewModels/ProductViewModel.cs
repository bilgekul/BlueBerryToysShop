using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace BlueBerry.ToysShop.Web.ViewModels
{
    public class ProductViewModel   {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string Description { get; set; }
        public string? Brand { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public double Rating { get; set; }
        public int Expire { get; set; }

        [ValidateNever]
        public IFormFile? Image { get; set; }

        [ValidateNever]
        public string? ImagePath { get; set; }

        public DateTime? PublishDate { get; set; }

        public int CategoryId { get; set; }

        public string? CategoryName { get; set; }
    }
}
