using ADTOSharp.Dependency;
using ADTOSharp.Resources.Embedded;

namespace ADTOSharp.AspNetCore.EmbeddedResources;

public class EmbeddedResourceViewFileProvider : EmbeddedResourceFileProvider
{
    public EmbeddedResourceViewFileProvider(IIocResolver iocResolver)
        : base(iocResolver)
    {
    }

    protected override bool IsIgnoredFile(EmbeddedResourceItem resource)
    {
        return resource.FileExtension != "cshtml";
    }
}