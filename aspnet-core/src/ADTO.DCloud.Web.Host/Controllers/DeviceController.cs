using ADTO.DCloud.Controllers;
using ADTO.DCloud.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading.Tasks;


namespace ADTO.DCloud.Web.Host.Controllers;


[Route("api/[controller]/[action]")]
public class DeviceController : DCloudControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly IDCloudFileProvider _fileProvider;

    public DeviceController(IWebHostEnvironment env, IDCloudFileProvider fileProvider)
    {
        _env = env;
        _fileProvider = fileProvider;
    }



    [HttpPost]
    public async Task<ActionResult> CreepJS([FromBody] JObject json)
    {
        string fullPath = Path.Combine(_env.WebRootPath, "creepjs", "fingerprint.json");
        if (!_fileProvider.DirectoryExists(Path.GetDirectoryName(fullPath)))
        {
            _fileProvider.CreateDirectory(Path.GetDirectoryName(fullPath));
        }
        await System.IO.File.AppendAllTextAsync(fullPath, $"{JsonConvert.SerializeObject(json,Formatting.None)}{System.Environment.NewLine}", System.Text.Encoding.UTF8);
        return Ok(true);
    }
}

