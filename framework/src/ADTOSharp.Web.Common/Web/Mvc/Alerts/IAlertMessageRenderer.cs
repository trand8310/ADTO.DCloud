using System;
using System.Collections.Generic;
using System.Text;
using ADTOSharp.Dependency;

namespace ADTOSharp.Web.Mvc.Alerts
{
    public interface IAlertMessageRenderer : ITransientDependency
    {
        string DisplayType { get; }
        string Render(List<AlertMessage> alertList);
    }
}
