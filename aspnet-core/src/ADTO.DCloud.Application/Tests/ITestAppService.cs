using System.Threading.Tasks;
using ADTOSharp.Application.Services;
using ADTO.DCloud.Tasks.Dto;
using System.Collections.Generic;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Domain.Uow;
using System.Transactions;
using System;

namespace ADTO.DCloud.Tests;

public interface ITestAppService : IApplicationService
{

}
