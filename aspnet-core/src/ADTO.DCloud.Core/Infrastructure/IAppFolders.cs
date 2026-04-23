namespace ADTO.DCloud.Infrastructure
{
    public interface IAppFolders
    {
        string SampleProfileImagesFolder { get; }

        string WebLogsFolder { get; set; }
    }
}