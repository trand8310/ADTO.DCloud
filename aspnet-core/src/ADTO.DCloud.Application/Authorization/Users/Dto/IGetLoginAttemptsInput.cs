using ADTOSharp.Application.Services.Dto;

namespace ADTO.DCloud.Authorization.Users.Dto
{
    public interface IGetLoginAttemptsInput: ISortedResultRequest
    {
        string Filter { get; set; }
    }
}