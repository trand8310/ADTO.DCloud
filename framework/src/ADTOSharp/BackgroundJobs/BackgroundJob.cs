namespace ADTOSharp.BackgroundJobs
{
    public abstract class BackgroundJob<TArgs> : BackgroundJobBase<TArgs>, IBackgroundJob<TArgs>
    {
        public abstract void Execute(TArgs args);
    }
}
