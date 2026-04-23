using JetBrains.Annotations;

namespace ADTOSharp.BlobStoring
{
    public interface IBlobProviderSelector
    {
        [NotNull]
        IBlobProvider Get([NotNull] string containerName);
    }
}