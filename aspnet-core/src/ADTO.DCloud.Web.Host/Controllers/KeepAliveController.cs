using ADTO.DCloud.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace ADTO.DCloud.Web.Host.Controllers;


public partial class KeepAliveController : DCloudControllerBase
{
    public virtual IActionResult Index()
    {
        return Content("I am alive!");
    }
}