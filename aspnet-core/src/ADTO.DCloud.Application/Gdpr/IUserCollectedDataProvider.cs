using System.Collections.Generic;
using System.Threading.Tasks;
using ADTOSharp;
using ADTO.DCloud.Dto;

namespace ADTO.DCloud.Gdpr
{
    public interface IUserCollectedDataProvider
    {
        Task<List<FileDto>> GetFiles(UserIdentifier user);
    }
}
