using ADTOSharp.BlobStoring;

namespace ADTOSharp.BlobStoring.FileSystem
{
    public interface IBlobFilePathCalculator
    {
        string Calculate(BlobProviderArgs args);
    }
}