using System.Threading.Tasks;

namespace ADTOSharp.BackgroundJobs
{
    public abstract class AsyncBackgroundJob<TArgs> : BackgroundJobBase<TArgs>, IAsyncBackgroundJob<TArgs>
    {
        public abstract Task ExecuteAsync(TArgs args);
    }
}