namespace ADTO.DistributedLocking;

public class ADTODistributedLockOptions
{
    /// <summary>
    /// DistributedLock key prefix.
    /// </summary>
    public string KeyPrefix  { get; set; }
    
    public ADTODistributedLockOptions()
    {
        KeyPrefix = "";
    }
}