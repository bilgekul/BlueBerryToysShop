namespace BlueBerry.ToysShop.Web.Models
{
	public class Visitor
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public int ProductId { get; set; }
		public decimal ProductRating { get; set; }	
		public string Name { get; set; }
		public string Comment { get; set; }
		public DateTime Created { get; set; }
	}
}
