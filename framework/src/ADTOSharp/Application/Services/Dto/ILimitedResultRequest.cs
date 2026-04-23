namespace ADTOSharp.Application.Services.Dto
{
    /// <summary>
    /// This interface is defined to standardize to request a limited result.
    /// </summary>
    public interface ILimitedResultRequest
    {
        /// <summary>
        ///  分页时最多可以返回的记录条数
        /// </summary>
        int PageSize { get; set; }
    }
}