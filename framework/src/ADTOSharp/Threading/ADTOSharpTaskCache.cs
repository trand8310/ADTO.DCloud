using System.Threading.Tasks;

namespace ADTOSharp.Threading
{
    public static class ADTOSharpTaskCache
    {
        public static Task CompletedTask { get; } = Task.FromResult(0);
    }
}
