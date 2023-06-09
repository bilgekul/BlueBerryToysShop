using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace BlueBerry.ToysShop.Web.ViewModels
{
    public class ProductViewModel   {
        public int Id { get; set; }

		[StringLength(50, ErrorMessage = "isim alanına en fazla 50 karakter girilebilir.")]
		[Required(ErrorMessage = "İsim alanı boş olamaz.")]
		public string? Name { get; set; }

		[StringLength(1000, MinimumLength = 50, ErrorMessage = "Açıklama alanı 50 ile 1000 karakter arasında olabilir.")]
		[Required(ErrorMessage = "Açıklama alanı boş olamaz.")]
		public string Description { get; set; }
        public string? Brand { get; set; }

		[Range(1, 1000, ErrorMessage = "Fiyat alanı 1 ile 1000 arasında bir değer olmalıdır.")]
		[Required(ErrorMessage = "Fiyat alanı boş olamaz.")]
		public decimal Price { get; set; }

		[Required(ErrorMessage = "Stok alanı boş olamaz.")]
		[Range(1, 2000, ErrorMessage = "Stok alanı 1 ile 2000 arasında bir değer olmalıdır.")]
		public int Quantity { get; set; }
        public double Rating { get; set; }

		[Required(ErrorMessage = "Yayınlanma süresi boş olamaz.")]
		public int Expire { get; set; }

        [ValidateNever]
        public IFormFile? Image { get; set; }

        [ValidateNever]
        public string? ImagePath { get; set; }

        public DateTime? PublishDate { get; set; }

		[Required(ErrorMessage = "Kategori seçiniz")]
		public int CategoryId { get; set; }

        public string? CategoryName { get; set; }
    }
}
