using System;
using System.Collections.Generic;
using System.Text;
using ADTOSharp.Dependency;

namespace ADTOSharp.Web.Mvc.Alerts
{
    public interface IAlertMessageRendererManager : ITransientDependency
    {
        string Render(string alertDisplayType);
    }
}
