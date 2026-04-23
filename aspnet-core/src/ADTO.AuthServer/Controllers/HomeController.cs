using Microsoft.AspNetCore.Mvc;
using ADTOSharp.AspNetCore.Mvc.Authorization;
using ADTO.DCloud.Controllers;

namespace ADTO.AuthServer.Controllers
{
    [ADTOSharpMvcAuthorize]
    public class HomeController : DCloudControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}

