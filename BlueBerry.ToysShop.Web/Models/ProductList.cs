using System.ComponentModel.DataAnnotations;

namespace BlueBerry.ToysShop.Web.Models
{
    public class ProductList
    {
        [Key]
        public int Id { get; set; }

        public string UserName { get; set; } 

        public List<ListItem> ListItems { get; set; }
    }
}
