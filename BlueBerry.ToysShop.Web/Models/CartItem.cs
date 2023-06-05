namespace BlueBerry.ToysShop.Web.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public Cart Cart { get; set; }
        public Product Product { get; set; }
        public string Osuruk { get; set; }
    }
}
