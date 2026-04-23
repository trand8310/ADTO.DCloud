using System;
using ADTOSharp.Auditing;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Extensions;
using ADTOSharp.Runtime.Validation;
using Microsoft.AspNetCore.Mvc;

namespace ADTOSharp.AspNetCore.Mvc.Controllers;

public class ADTOSharpAppViewController : ADTOSharpController
{
    [DisableAuditing]
    [DisableValidation]
    [UnitOfWork(IsDisabled = true)]
    public ActionResult Load(string viewUrl)
    {
        if (viewUrl.IsNullOrEmpty())
        {
            throw new ArgumentNullException(nameof(viewUrl));
        }

        return View(viewUrl.EnsureStartsWith('~'));
    }
}