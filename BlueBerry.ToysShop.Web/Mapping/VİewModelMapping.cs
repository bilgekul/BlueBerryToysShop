using AutoMapper;
using BlueBerry.ToysShop.Web.Models;
using BlueBerry.ToysShop.Web.ViewModels;

namespace BlueBerry.ToysShop.Web.Mapping
{
    public class VİewModelMapping:Profile
    {
        public VİewModelMapping()
        {
            CreateMap<Product,ProductViewModel>().ReverseMap();
            CreateMap<Product,ProductUpdateViewModel>().ReverseMap();
            CreateMap<Customer,CustomerViewModel>().ReverseMap();
        }
    }
}
