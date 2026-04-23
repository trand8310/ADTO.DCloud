namespace ADTOSharp.Auditing
{
    public interface IAuditSerializer
    {
        string Serialize(object obj);
    }
}