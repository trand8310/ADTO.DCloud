namespace ADTO.DCloud.Desktop.Services;

public interface IClockService
{
    DateTimeOffset Now { get; }
}
