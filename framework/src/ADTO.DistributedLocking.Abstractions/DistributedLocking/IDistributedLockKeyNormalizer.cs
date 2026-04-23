namespace ADTO.DistributedLocking;

public interface IDistributedLockKeyNormalizer
{
    string NormalizeKey(string name);
    
}