using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlueBerry.ToysShop.Web.Controllers
{
    public class ClaimBasedController : Controller
    {
        [Authorize(Policy = "HrDepartmentPolicy")]
        public IActionResult HR() => View();

        [Authorize(Policy = "SalesDepartmentPolicy")]
        public IActionResult Software() => View();

        [Authorize(Policy = "EmployeePolicy")]
        public IActionResult Employee() => View();

        [Authorize(Policy = "AtLeast18Policy")]
        public IActionResult Violence() => View();
    }
}
