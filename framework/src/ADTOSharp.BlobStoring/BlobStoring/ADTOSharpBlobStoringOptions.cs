namespace ADTOSharp.BlobStoring
{
    public class ADTOSharpBlobStoringOptions
    {
        public BlobContainerConfigurations Containers { get; }

        public ADTOSharpBlobStoringOptions()
        {
            Containers = new BlobContainerConfigurations();
        }
    }
}
