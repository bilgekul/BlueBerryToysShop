namespace BlueBerry.ToysShop.Web.ViewModels
{
	public class ProductFilterViewModel
	{
		public string ProductName { get; set; }
		public decimal? MinPrice { get; set; }
		public decimal? MaxPrice { get; set; }
		public List<string> Brands { get; set; }
		public List<int> CategoryIds { get; set; }
	}
}
