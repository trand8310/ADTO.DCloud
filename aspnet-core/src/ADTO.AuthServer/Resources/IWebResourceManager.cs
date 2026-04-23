using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;

namespace ADTO.AuthServer.Resources;

public interface IWebResourceManager
{
    void AddScript(string url, bool addMinifiedOnProd = true);

    IReadOnlyList<string> GetScripts();

    HelperResult RenderScripts();
}
