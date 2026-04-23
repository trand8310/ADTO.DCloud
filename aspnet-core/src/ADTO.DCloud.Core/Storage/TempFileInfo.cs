namespace ADTO.DCloud.Storage;

/// <summary>
/// 临时文件信息
/// </summary>
public class TempFileInfo
{
    public string FileName { get; set; }
    public string FileType { get; set; }
    public byte[] File { get; set; }

    /// <summary>
    /// 附件原名
    /// </summary>
    public string OriginalName { get; set; }

    /// <summary>
    /// 文件大小
    /// </summary>
    public long Length { get; set; }

    public TempFileInfo()
    {
    }

    public TempFileInfo(byte[] file)
    {
        File = file;
    }

    public TempFileInfo(string fileName, string fileType, byte[] file)
    {
        FileName = fileName;
        FileType = fileType;
        File = file;
    }
}