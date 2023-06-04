namespace BlueBerry.ToysShop.Web.ViewModels
{
    public class VisitorViewModel
    {
		public int Id { get; set; }
		public string UserId { get; set; }
		public int ProductId { get; set; }
        public int ProductRating { get; set; }
        public string Name { get; set; }
		public string Comment { get; set; }
		public DateTime Created { get; set; }
	}
}
