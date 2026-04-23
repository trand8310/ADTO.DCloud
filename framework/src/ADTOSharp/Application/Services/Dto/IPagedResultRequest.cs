namespace ADTOSharp.Application.Services.Dto
{
    /// <summary>
    /// This interface is defined to standardize to request a paged result.
    /// </summary>
    public interface IPagedResultRequest : ILimitedResultRequest
    {
        /// <summary>
        /// 分页时用于记录当前页码
        /// </summary>
        int PageNumber { get; set; }
    }
}