using ADTOSharp.Modules;
using ADTOSharp.Reflection.Extensions;

namespace ADTO.DistributedLocking;

public class ADTODistributedLockingAbstractionsModule : ADTOSharpModule
{

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(ADTODistributedLockingAbstractionsModule).GetAssembly());
    }
}
