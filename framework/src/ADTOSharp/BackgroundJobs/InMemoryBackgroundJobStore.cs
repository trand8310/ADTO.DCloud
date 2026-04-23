using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ADTOSharp.Dependency;
using ADTOSharp.Timing;

namespace ADTOSharp.BackgroundJobs
{
    /// <summary>
    /// In memory implementation of <see cref="IBackgroundJobStore"/>.
    /// It's used if <see cref="IBackgroundJobStore"/> is not implemented by actual persistent store
    /// and job execution is enabled (<see cref="IBackgroundJobConfiguration.IsJobExecutionEnabled"/>) for the application.
    /// </summary>
    public class InMemoryBackgroundJobStore : IBackgroundJobStore
    {
        private readonly ConcurrentDictionary<Guid, BackgroundJobInfo> _jobs;
        private readonly IGuidGenerator _guidGenerator;

        //private long _lastId;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryBackgroundJobStore"/> class.
        /// </summary>
        public InMemoryBackgroundJobStore()
        {
            _jobs = new ConcurrentDictionary<Guid, BackgroundJobInfo>();
            _guidGenerator = IocManager.Instance.Resolve<IGuidGenerator>();
        }

        public Task<BackgroundJobInfo> GetAsync(Guid jobId)
        {
            return Task.FromResult(_jobs[jobId]);
        }

        public BackgroundJobInfo Get(Guid jobId)
        {
            return _jobs[jobId];
        }

        public Task InsertAsync(BackgroundJobInfo jobInfo)
        {
            jobInfo.Id = _guidGenerator.Create();
            _jobs[jobInfo.Id] = jobInfo;

            return Task.FromResult(0);
        }

        public void Insert(BackgroundJobInfo jobInfo)
        {
            jobInfo.Id = _guidGenerator.Create();
            _jobs[jobInfo.Id] = jobInfo;
        }

        public Task<List<BackgroundJobInfo>> GetWaitingJobsAsync(int maxResultCount)
        {
            var waitingJobs = _jobs.Values
                .Where(t => !t.IsAbandoned && t.NextTryTime <= Clock.Now)
                .OrderByDescending(t => t.Priority)
                .ThenBy(t => t.TryCount)
                .ThenBy(t => t.NextTryTime)
                .Take(maxResultCount)
                .ToList();

            return Task.FromResult(waitingJobs);
        }

        public List<BackgroundJobInfo> GetWaitingJobs(int maxResultCount)
        {
            var waitingJobs = _jobs.Values
                .Where(t => !t.IsAbandoned && t.NextTryTime <= Clock.Now)
                .OrderByDescending(t => t.Priority)
                .ThenBy(t => t.TryCount)
                .ThenBy(t => t.NextTryTime)
                .Take(maxResultCount)
                .ToList();

            return waitingJobs;
        }

        public Task DeleteAsync(BackgroundJobInfo jobInfo)
        {
            _jobs.TryRemove(jobInfo.Id, out _);

            return Task.FromResult(0);
        }

        public void Delete(BackgroundJobInfo jobInfo)
        {
            _jobs.TryRemove(jobInfo.Id, out _);
        }

        public Task UpdateAsync(BackgroundJobInfo jobInfo)
        {
            if (jobInfo.IsAbandoned)
            {
                return DeleteAsync(jobInfo);
            }

            return Task.FromResult(0);
        }

        public void Update(BackgroundJobInfo jobInfo)
        {
            if (jobInfo.IsAbandoned)
            {
                Delete(jobInfo);
            }
        }
    }
}