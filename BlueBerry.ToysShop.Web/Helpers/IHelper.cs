using BlueBerry.ToysShop.Web.Database_Settings;

namespace MyAspNetCore.Web.Helpers
{
	public interface IHelper
	{
		public bool ValidCredentials(string email, string password, WebDbContext context);
	}
}
