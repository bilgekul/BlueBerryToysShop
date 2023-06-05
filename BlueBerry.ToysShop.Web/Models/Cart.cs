namespace BlueBerry.ToysShop.Web.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public List<CartItem> CartItems { get; set; }
    }
}
