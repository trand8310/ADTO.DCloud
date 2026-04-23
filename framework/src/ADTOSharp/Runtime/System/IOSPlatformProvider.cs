using System.Runtime.InteropServices;

namespace ADTOSharp.Runtime.System
{
    public interface IOSPlatformProvider
    {
        OSPlatform GetCurrentOSPlatform();
    }
}