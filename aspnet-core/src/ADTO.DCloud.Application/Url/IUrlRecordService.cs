using System.Threading.Tasks;

namespace ADTO.DCloud.Url
{
    public partial interface IUrlRecordService
    {
        Task<string> GetSeNameAsync(string name, bool convertNonWesternChars, bool allowUnicodeCharsInUrls);
    }
}