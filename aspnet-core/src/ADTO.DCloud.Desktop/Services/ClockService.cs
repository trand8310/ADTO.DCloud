namespace ADTO.DCloud.Desktop.Services;

public sealed class ClockService : IClockService
{
    public DateTimeOffset Now => DateTimeOffset.Now;
}
