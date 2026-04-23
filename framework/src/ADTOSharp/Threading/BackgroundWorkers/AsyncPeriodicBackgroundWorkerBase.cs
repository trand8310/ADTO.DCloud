using System;
using System.Threading.Tasks;
using ADTOSharp.Threading.Timers;

namespace ADTOSharp.Threading.BackgroundWorkers
{
    public abstract class AsyncPeriodicBackgroundWorkerBase : BackgroundWorkerBase
    {
        protected readonly ADTOSharpAsyncTimer Timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="PeriodicBackgroundWorkerBase"/> class.
        /// </summary>
        /// <param name="timer">A timer.</param>
        protected AsyncPeriodicBackgroundWorkerBase(ADTOSharpAsyncTimer timer)
        {
            Timer = timer;
            Timer.Elapsed += Timer_Elapsed;
        }

        public override void Start()
        {
            base.Start();
            Timer.Start();
        }

        public override void Stop()
        {
            Timer.Stop();
            base.Stop();
        }

        public override void WaitToStop()
        {
            Timer.WaitToStop();
            base.WaitToStop();
        }

        /// <summary>
        /// Handles the Elapsed event of the Timer.
        /// </summary>
        private async Task Timer_Elapsed(ADTOSharpAsyncTimer timer)
        {
            try
            {
                await DoWorkAsync();
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.ToString(), ex);
            }
        }

        /// <summary>
        /// Periodic works should be done by implementing this method.
        /// </summary>
        protected abstract Task DoWorkAsync();
    }
}
