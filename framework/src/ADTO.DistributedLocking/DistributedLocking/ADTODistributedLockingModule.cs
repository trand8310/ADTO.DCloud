using ADTOSharp.Modules;
using ADTOSharp.Reflection.Extensions;


namespace ADTO.DistributedLocking;

[DependsOn(
    typeof(ADTODistributedLockingAbstractionsModule)
    )]
public class ADTODistributedLockingModule : ADTOSharpModule
{

    public override void PreInitialize()
    {

       
 
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(ADTODistributedLockingModule).GetAssembly());
    }
}