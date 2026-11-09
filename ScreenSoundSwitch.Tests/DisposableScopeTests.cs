using System;
using System.Threading;
using SoundSwitch.Common.Framework.Dispose;
using Xunit;

namespace ScreenSoundSwitch.Tests
{
    public class DisposableScopeTests
    {
        [Fact]
        public void DisposableScope_RunsActionOnDispose()
        {
            var runs = 0;

            using (new DisposableScope(() => runs++))
            {
            }

            Assert.Equal(1, runs);
        }

        [Fact]
        public void CancellableDisposableScope_DisposeTwice_RunsActionOnce()
        {
            var runs = 0;
            var scope = new CancellableDisposableScope(() => runs++, CancellationToken.None);

            scope.Dispose();
            scope.Dispose();

            Assert.Equal(1, runs);
        }

        [Fact]
        public void CancellableDisposableScope_Cancellation_RunsAction()
        {
            var runs = 0;
            using var source = new CancellationTokenSource();
            var scope = new CancellableDisposableScope(() => runs++, source.Token);

            source.Cancel();

            Assert.Equal(1, runs);

            scope.Dispose();

            Assert.Equal(1, runs);
        }

        [Fact]
        public void DisposeOnCancellation_Dispose_DisposesOnce()
        {
            var disposable = new CountingDisposable();
            var scope = disposable.DisposeOnCancellation(CancellationToken.None);

            scope.Dispose();
            scope.Dispose();

            Assert.Equal(1, disposable.DisposeCount);
        }

        [Fact]
        public void DisposeOnCancellation_Cancellation_DisposesOnce()
        {
            var disposable = new CountingDisposable();
            using var source = new CancellationTokenSource();
            var scope = disposable.DisposeOnCancellation(source.Token);

            source.Cancel();
            scope.Dispose();

            Assert.Equal(1, disposable.DisposeCount);
        }

        [Fact]
        public void DisposeOnCancellation_AlreadyCancelledToken_DisposesImmediately()
        {
            var disposable = new CountingDisposable();
            using var source = new CancellationTokenSource();
            source.Cancel();

            using (disposable.DisposeOnCancellation(source.Token))
            {
            }

            Assert.Equal(1, disposable.DisposeCount);
        }

        private sealed class CountingDisposable : IDisposable
        {
            public int DisposeCount { get; private set; }

            public void Dispose()
            {
                DisposeCount++;
            }
        }
    }
}
