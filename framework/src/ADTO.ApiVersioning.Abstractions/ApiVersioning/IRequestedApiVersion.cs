namespace ADTO.ApiVersioning;

public interface IRequestedApiVersion
{
    string? Current { get; }
}
