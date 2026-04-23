using ADTOSharp.Dependency;

namespace ADTOSharp.Application.Services
{
    /// <summary>
    /// 该接口必须由所有应用程序服务实现，以便按照约定识别它们。
    /// </summary>
    public interface IApplicationService : ITransientDependency
    {

    }
}
