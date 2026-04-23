using ADTOSharp;
using ADTOSharp.Domain.Services;
using System.Threading.Tasks;

namespace ADTO.DCloud.Authorization.Users.Profile
{
    public interface IProfileImageService : IDomainService
    {
        Task<string> GetProfilePictureContentForUser(UserIdentifier userIdentifier);
    }
}