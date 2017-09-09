using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFLibs.Core.Basis
{
    public sealed class AsyncLock
    {
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        private readonly Task<IDisposable> releaser;

        public AsyncLock()
        {
            this.releaser = Task.FromResult((IDisposable)new Releaser(this));
        }

        public Task<IDisposable> LockAsync()
        {
            var wait = this.semaphore.WaitAsync();
            return wait.IsCompleted ?
                    this.releaser :
                    wait.ContinueWith(
                      (_, state) => (IDisposable)state,
                      this.releaser.Result,
                      CancellationToken.None,
                      TaskContinuationOptions.ExecuteSynchronously,
                      TaskScheduler.Default
                    );
        }

        private sealed class Releaser : IDisposable
        {
            private readonly AsyncLock asyncLock;

            internal Releaser(AsyncLock toRelease)
            {
                this.asyncLock = toRelease;
            }

            public void Dispose()
            {
                this.asyncLock.semaphore.Release();
            }
        }
    }
}
