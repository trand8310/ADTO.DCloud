using System.ComponentModel.DataAnnotations;

namespace ADTOSharp.Application.Services.Dto
{
    /// <summary>
    /// Simply implements <see cref="ILimitedResultRequest"/>.
    /// </summary>
    public class LimitedResultRequestDto : ILimitedResultRequest
    {
        public static int DefaultMaxPageSize { get; set; } = 10;

        [Range(1, int.MaxValue)]
        public virtual int PageSize { get; set; } = DefaultMaxPageSize;
    }
}