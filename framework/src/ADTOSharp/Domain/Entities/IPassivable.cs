namespace ADTOSharp.Domain.Entities
{
    /// <summary>
    /// 记录是有效
    /// </summary>
    public interface IPassivable
    {
        /// <summary>
        /// True: This entity is active.
        /// False: This entity is not active.
        /// </summary>
        bool IsActive { get; set; }
    }
}